﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace AcornSharp
{
    [SuppressMessage("ReSharper", "LocalVariableHidesMember")]
    public sealed partial class Parser
    {
        // Object type used to represent tokens. Note that normally, tokens
        // simply exist as properties on the parser object. This is only
        // used for the onToken callback and the external tokenizer.

        private sealed class Token
        {
            private TokenType type;
            private object value;
            private int start;
            private int end;
            private SourceLocation loc;
            private (int, int) range;

            public Token(Parser p)
            {
                type = p.type;
                value = p.value;
                start = p.start;
                end = p.end;
                loc = new SourceLocation(p, p.startLoc, p.endLoc);
                range = (p.start, p.end);
            }
        }

        // Move to the next token
        private void next()
        {
            if (Options.onToken != null)
            {
//                this.options.onToken(new Token(this))
                throw new NotImplementedException();
            }

            lastTokEnd = end;
            lastTokStart = start;
            lastTokEndLoc = endLoc;
            lastTokStartLoc = startLoc;
            nextToken();
        }

        private Token getToken()
        {
            next();
            return new Token(this);
        }

        // If we're in an ES6 environment, make parsers iterable
        //if (typeof Symbol !== "undefined")
        //  pp[Symbol.iterator] = function() {
        //    return {
        //      next: () => {
        //        let token = this.getToken()
        //        return {
        //          done: token.type === tt.eof,
        //          value: token
        //        }
        //      }
        //    }
        //  }

        // Toggle strict mode. Re-reads the next number or string to please
        // pedantic tests (`"use strict"; 010;` should fail).
        private TokContext curContext()
        {
            return context[context.Count - 1];
        }

        // Read a single token, updating the parser object's token-related
        // properties.
        private void nextToken()
        {
            var curContext = this.curContext();
            if (curContext == null || curContext.PreserveSpace != true) skipSpace();

            start = pos;
            startLoc = curPosition();
            if (pos >= input.Length)
            {
                finishToken(TokenType.eof);
                return;
            }

            if (curContext?.Override != null) curContext.Override(this);
            else readToken(fullCharCodeAtPos());
        }

        private void readToken(int code)
        {
            // Identifier or keyword. '\uXXXX' sequences are allowed in
            // identifiers, so '\' also dispatches to that.
            if (isIdentifierStart(code, Options.ecmaVersion >= 6) || code == 92 /* '\' */)
            {
                readWord();
                return;
            }

            getTokenFromCode(code);
        }

        private int fullCharCodeAtPos()
        {
            if (pos >= input.Length)
                return 0;

            return char.ConvertToUtf32(input, pos);
        }

        private void skipBlockComment()
        {
            var startLoc = Options.onComment != null && curPosition() != null;
            var start = pos;
            var end = input.IndexOf("*/", pos += 2, StringComparison.Ordinal);
            if (end == -1) raise(pos - 2, "Unterminated comment");
            pos = end + 2;
            var lastIndex = start;
            while (true)
            {
                var match = lineBreak.Match(input, lastIndex);
                if (!match.Success || match.Index >= pos)
                    break;

                ++curLine;
                lineStart = match.Index + match.Length;
                lastIndex = lineStart;
            }
            Options.onComment?.Invoke(true, input.Substring(start + 2, end - (start + 2)), start, pos, startLoc, curPosition());
        }

        private void skipLineComment(int startSkip)
        {
            var start = pos;
            var startLoc = Options.onComment != null && curPosition() != null;
            var ch = input.Get(pos += startSkip);
            while (pos < input.Length && !isNewLine(ch))
            {
                ch = input.Get(++pos);
            }
            Options.onComment?.Invoke(false, input.Substring(start + startSkip, pos - (start + startSkip)), start, pos, startLoc, curPosition());
        }

        // Called at the start of the parse and after every token. Skips
        // whitespace and comments, and.
        private void skipSpace()
        {
            while (pos < input.Length)
            {
                var ch = (int)input[pos];
                switch (ch)
                {
                    case 32:
                    case 160: // ' '
                        ++pos;
                        break;
                    case 13:
                        if (input[pos + 1] == 10)
                            ++pos;
                        goto case 10;
                    case 10:
                    case 8232:
                    case 8233:
                        ++pos;
                        ++curLine;
                        lineStart = pos;
                        break;
                    case 47: // '/'
                        switch ((int)input.Get(pos + 1))
                        {
                            case 42: // '*'
                                skipBlockComment();
                                break;
                            case 47:
                                skipLineComment(2);
                                break;
                            default:
                                return;
                        }
                        break;
                    default:
                        if (ch > 8 && ch < 14 || ch >= 5760 && nonASCIIwhitespace.IsMatch(((char)ch).ToString()))
                        {
                            ++pos;
                            break;
                        }
                        else
                        {
                            return;
                        }
                }
            }
        }

        // Called at the end of every token. Sets `end`, `val`, and
        // maintains `context` and `exprAllowed`, and skips the space after
        // the token, so that the next one's `start` will point at the
        // right position.
        private void finishToken(TokenType type, object val = null)
        {
            end = pos;
            endLoc = curPosition();
            var prevType = this.type;
            this.type = type;
            value = val;

            updateContext(prevType);
        }

        // ### Token reading

        // This is the function that is called to fetch the next token. It
        // is somewhat obscure, because it works in character codes rather
        // than characters, and because operator parsing has been inlined
        // into it.
        //
        // All in the name of speed.
        //
        private void readToken_dot()
        {
            var next = input[pos + 1];
            if (next >= 48 && next <= 57)
            {
                readNumber(true);
                return;
            }
            var next2 = input[pos + 2];
            if (Options.ecmaVersion >= 6 && next == 46 && next2 == 46)
            {
                // 46 = dot '.'
                pos += 3;
                finishToken(TokenType.ellipsis);
            }
            else
            {
                ++pos;
                finishToken(TokenType.dot);
            }
        }

        private void readToken_slash() { // '/'
            var next = input.Get(pos + 1);
            if (exprAllowed)
            {
                ++pos;
                readRegexp();
                return;
            }
            if (next == 61) finishOp(TokenType.assign, 2);
            else finishOp(TokenType.slash, 1);
        }

        private void readToken_mult_modulo_exp(int code)
        {
            // '%*'
            var next = input.Get(pos + 1);
            var size = 1;
            var tokentype = code == 42 ? TokenType.star : TokenType.modulo;

            // exponentiation operator ** and **=
            if (Options.ecmaVersion >= 7 && code == 42 && next == 42)
            {
                ++size;
                tokentype = TokenType.starstar;
                next = input[pos + 2];
            }

            if (next == 61) finishOp(TokenType.assign, size + 1);
            else finishOp(tokentype, size);
        }

        private void readToken_pipe_amp(int code)
        {
            // '|&'
            var next = input[pos + 1];
            if (next == code) finishOp(code == 124 ? TokenType.logicalOR : TokenType.logicalAND, 2);
            else if (next == 61) finishOp(TokenType.assign, 2);
            else finishOp(code == 124 ? TokenType.bitwiseOR : TokenType.bitwiseAND, 1);
        }

        private void readToken_caret()
        {
            // '^'
            var next = input[pos + 1];
            if (next == 61) finishOp(TokenType.assign, 2);
            else finishOp(TokenType.bitwiseXOR, 1);
        }

        private void readToken_plus_min(int code)
        {
            // '+-'
            var next = input[pos + 1];
            if (next == code)
            {
                if (next == 45 && !inModule && input.Get(pos + 2) == 62 &&
                    (lastTokEnd == 0 || lineBreak.IsMatch(input.Substring(lastTokEnd, pos - lastTokEnd))))
                {
                    // A `-->` line comment
                    skipLineComment(3);
                    skipSpace();
                    nextToken();
                    return;
                }
                finishOp(TokenType.incDec, 2);
            }
            else if (next == 61) finishOp(TokenType.assign, 2);
            else finishOp(TokenType.plusMin, 1);
        }

        private void readToken_lt_gt(int code)
        {
            // '<>'
            var next = input[pos + 1];
            var size = 1;
            if (next == code)
            {
                size = code == 62 && input[pos + 2] == 62 ? 3 : 2;
                if (input[pos + size] == 61) finishOp(TokenType.assign, size + 1);
                else finishOp(TokenType.bitShift, size);
                return;
            }
            if (next == 33 && code == 60 && !inModule && input[pos + 2] == 45 &&
                input[pos + 3] == 45)
            {
                // `<!--`, an XML-style comment that should be interpreted as a line comment
                skipLineComment(4);
                skipSpace();
                nextToken();
                return;
            }
            if (next == 61) size = 2;
            finishOp(TokenType.relational, size);
        }

        private void readToken_eq_excl(int code)
        {
            // '=!'
            var next = input[pos + 1];
            if (next == 61) finishOp(TokenType.equality, input[pos + 2] == 61 ? 3 : 2);
            else if (code == 61 && next == 62 && Options.ecmaVersion >= 6)
            {
                // '=>'
                pos += 2;
                finishToken(TokenType.arrow);
            }
            else finishOp(code == 61 ? TokenType.eq : TokenType.prefix, 1);
        }

        private void getTokenFromCode(int code)
        {
            switch (code)
            {
                // The interpretation of a dot depends on whether it is followed
                // by a digit or another two dots.
                case 46: // '.'
                    readToken_dot();
                    return;

                // Punctuation tokens.
                case 40:
                    ++pos;
                    finishToken(TokenType.parenL);
                    return;
                case 41:
                    ++pos;
                    finishToken(TokenType.parenR);
                    return;
                case 59:
                    ++pos;
                    finishToken(TokenType.semi);
                    return;
                case 44:
                    ++pos;
                    finishToken(TokenType.comma);
                    return;
                case 91:
                    ++pos;
                    finishToken(TokenType.bracketL);
                    return;
                case 93:
                    ++pos;
                    finishToken(TokenType.bracketR);
                    return;
                case 123:
                    ++pos;
                    finishToken(TokenType.braceL);
                    return;
                case 125:
                    ++pos;
                    finishToken(TokenType.braceR);
                    return;
                case 58:
                    ++pos;
                    finishToken(TokenType.colon);
                    return;
                case 63:
                    ++pos;
                    finishToken(TokenType.question);
                    return;

                case 96: // '`'
                    if (Options.ecmaVersion < 6) break;
                    ++pos;
                    finishToken(TokenType.backQuote);
                    return;

                case 48: // '0'
                    var next = input.Get(pos + 1);
                    if (next == 120 || next == 88)
                    {
                        readRadixNumber(16); // '0x', '0X' - hex number
                        return;
                    }
                    if (Options.ecmaVersion >= 6)
                    {
                        if (next == 111 || next == 79) {
                            readRadixNumber(8); // '0o', '0O' - octal number
                            return;
                        }
                        if (next == 98 || next == 66) {
                            readRadixNumber(2); // '0b', '0B' - binary number
                            return;
                        }
                    }
                    goto case 49;
                // Anything else beginning with a digit is an integer, octal
                // number, or float.
                case 49:
                case 50:
                case 51:
                case 52:
                case 53:
                case 54:
                case 55:
                case 56:
                case 57: // 1-9
                    readNumber(false);
                    return;

                // Quotes produce strings.
                case 34:
                case 39: // '"', "'"
                    readString(code);
                    return;

                // Operators are parsed inline in tiny state machines. '=' (61) is
                // often referred to. `finishOp` simply skips the amount of
                // characters it is given as second argument, and returns a token
                // of the type given by its first argument.

                case 47: // '/'
                    readToken_slash();
                    return;

                case 37:
                case 42: // '%*'
                    readToken_mult_modulo_exp(code);
                    return;

                case 124:
                case 38: // '|&'
                    readToken_pipe_amp(code);
                    return;

                case 94: // '^'
                    readToken_caret();
                    return;

                case 43:
                case 45: // '+-'
                    readToken_plus_min(code);
                    return;

                case 60:
                case 62: // '<>'
                    readToken_lt_gt(code);
                    return;

                case 61:
                case 33: // '=!'
                    readToken_eq_excl(code);
                    return;

                case 126: // '~'
                    finishOp(TokenType.prefix, 1);
                    return;
            }

            raise(pos, "Unexpected character '" + codePointToString(code) + "'");
        }

        private void finishOp(TokenType type, int size)
        {
            var str = input.Substring(pos, size);
            pos += size;
            finishToken(type, str);
        }

        private void readRegexp()
        {
            var escaped = false;
            var inClass = false;
            var start = pos;
            while (true)
            {
                if (pos >= input.Length) raise(start, "Unterminated regular expression");
                var ch = input[pos];
                if (lineBreak.IsMatch(ch.ToString())) raise(start, "Unterminated regular expression");
                if (!escaped)
                {
                    if (ch == '[') inClass = true;
                    else if (ch == ']' && inClass) inClass = false;
                    else if (ch == '/' && !inClass) break;
                    escaped = ch == '\\';
                }
                else escaped = false;
                ++pos;
            }

            var content = input.Substring(start, pos - start);
            ++pos;
            // Need to use `readWord1` because '\uXXXX' sequences are allowed
            // here (don't ask).
            var mods = readWord1();
            if (!string.IsNullOrEmpty(mods))
            {
                var validFlags = new Regex("^[gim]*$");
                if (Options.ecmaVersion >= 6) validFlags = new Regex("^[gimuy]*$");
                if (!validFlags.IsMatch(mods)) raise(start, "Invalid regular expression flag");
            }
            finishToken(TokenType.regexp, new RegexNode
            {
                pattern = content,
                flags = mods
            });
        }

        // Read an integer in the given radix. Return null if zero digits
        // were read, the integer value otherwise. When `len` is given, this
        // will return `null` unless the integer has exactly `len` digits.
        private int? readInt(int radix, int? len = null)
        {
            var total = 0;
            var start = pos;
            for (var i = 0; !len.HasValue || i < len; ++i)
            {
                var code = input.Get(pos);
                int val;
                if (code >= 97) val = code - 97 + 10; // a
                else if (code >= 65) val = code - 65 + 10; // A
                else if (code >= 48 && code <= 57) val = code - 48; // 0-9
                else val = int.MaxValue;
                if (val >= radix) break;
                ++pos;
                total = total * radix + val;
            }

            if (pos == start || len != null && pos - start != len) return null;
            return total;
        }

        private void readRadixNumber(int radix)
        {
            pos += 2; // 0x
            var val = readInt(radix);
            if (!val.HasValue)
            {
                raise(start + 2, "Expected number in radix " + radix);
                return;
            }
            if (isIdentifierStart(fullCharCodeAtPos())) raise(pos, "Identifier directly after number");
            finishToken(TokenType.num, val.Value);
        }

        // Read an integer, octal integer, or floating-point number.
        private void readNumber(bool startsWithDot)
        {
            var start = pos;
            var isFloat = false;
            var octal = input[pos] == 48;
            if (!startsWithDot && readInt(10) == null) raise(start, "Invalid number");
            if (octal && pos == start + 1) octal = false;
            var next = input.Get(pos);

            if (next == 46 && !octal)
            {
                // '.'
                ++pos;
                readInt(10);
                isFloat = true;
                next = input.Get(pos);
            }

            if ((next == 69 || next == 101) && !octal)
            {
                // 'eE'
                next = input.Get(++pos);
                if (next == 43 || next == 45) ++pos; // '+-'
                if (readInt(10) == null) raise(start, "Invalid number");
                isFloat = true;
            }
            if (isIdentifierStart(fullCharCodeAtPos())) raise(pos, "Identifier directly after number");

            var str = input.Substring(start, pos - start);
            object val;
            if (isFloat) val = double.Parse(str);
            else if (!octal || str.Length == 1) val = parseInt(str, 10);
            else if (strict)
            {
                raise(start, "Invalid number");
                return;
            }
            else if (Regex.IsMatch(str, "[89]")) val = parseInt(str, 10);
            else val = parseInt(str, 8);
            finishToken(TokenType.num, val);
        }

        // Read a string value, interpreting backslash-escapes.
        private int readCodePoint()
        {
            var ch = input.Get(pos);
            int code;

            if (ch == 123)
            {
                // '{'
                if (Options.ecmaVersion < 6) unexpected();
                var codePos = ++pos;
                code = readHexChar(input.IndexOf("}", pos, StringComparison.Ordinal) - pos);
                ++pos;
                if (code > 0x10FFFF) invalidStringToken(codePos, "Code point out of bounds");
            }
            else
            {
                code = readHexChar(4);
            }
            return code;
        }

        private string codePointToString(int code)
        {
            return char.ConvertFromUtf32(code);
        }

        private void readString(int quote)
        {
            var @out = "";
            var chunkStart = ++pos;
            while (true)
            {
                if (pos >= input.Length) raise(start, "Unterminated string constant");
                var ch = input[pos];
                if (ch == quote) break;
                if (ch == 92)
                {
                    // '\'
                    @out += input.Substring(chunkStart, pos - chunkStart);
                    @out += readEscapedChar(false);
                    chunkStart = pos;
                }
                else
                {
                    if (isNewLine(ch)) raise(start, "Unterminated string constant");
                    ++pos;
                }
            }
            @out += input.Substring(chunkStart, pos - chunkStart);
            pos++;
            finishToken(TokenType.@string, @out);
        }

        // Reads template string tokens.
        private sealed class INVALID_TEMPLATE_ESCAPE_ERROR : Exception
        {
        }

        public void tryReadTemplateToken()
        {
            inTemplateElement = true;
            try
            {
                readTmplToken();
            }
            catch (INVALID_TEMPLATE_ESCAPE_ERROR)
            {
                readInvalidTemplateToken();
            }

            inTemplateElement = false;
        }

        private void invalidStringToken(int position, string message)
        {
            if (inTemplateElement && Options.ecmaVersion >= 9)
            {
                throw new INVALID_TEMPLATE_ESCAPE_ERROR();
            }

            raise(position, message);
        }

        private void readTmplToken()
        {
            var @out = "";
            var chunkStart = pos;
            while (true)
            {
                if (pos >= input.Length) raise(start, "Unterminated template");
                var ch = input[pos];
                if (ch == 96 || ch == 36 && input[pos + 1] == 123)
                {
                    // '`', '${'
                    if (pos == start && (type == TokenType.template || type == TokenType.invalidTemplate))
                    {
                        if (ch == 36)
                        {
                            pos += 2;
                            finishToken(TokenType.dollarBraceL);
                            return;
                        }

                        ++pos;
                        finishToken(TokenType.backQuote);
                        return;
                    }
                    @out += input.Substring(chunkStart, pos - chunkStart);
                    finishToken(TokenType.template, @out);
                    return;
                }
                if (ch == 92)
                {
                    // '\'
                    @out += input.Substring(chunkStart, pos - chunkStart);
                    @out += readEscapedChar(true);
                    chunkStart = pos;
                }
                else if (isNewLine(ch))
                {
                    @out += input.Substring(chunkStart, pos - chunkStart);
                    ++pos;
                    switch ((int)ch)
                    {
                        case 13:
                            if (input[pos] == 10) ++pos;
                            goto case 10;
                        case 10:
                            @out += "\n";
                            break;
                        default:
                            @out += ch.ToString();
                            break;
                    }
                    ++curLine;
                    lineStart = pos;
                    chunkStart = pos;
                }
                else
                {
                    ++pos;
                }
            }
        }
        // Reads a template token to search for the end, without validating any escape sequences
        private void readInvalidTemplateToken()
        {
            for (; pos < input.Length; pos++)
            {
                switch (input[pos])
                {
                    case '\\':
                        ++pos;
                        break;

                    case '$':
                        if (input[pos + 1] != '{')
                        {
                            break;
                        }
                        goto case '`';
                    // falls through

                    case '`':
                        finishToken(TokenType.invalidTemplate, input.Substring(start, pos - start));
                        return;

                    // no default
                }
            }
            raise(start, "Unterminated template");
        }

        // Used to read escaped characters
        private string readEscapedChar(bool inTemplate)
        {
            var ch = input.Get(++pos);
            ++pos;
            switch ((int)ch)
            {
                case 110: return "\n"; // 'n' -> '\n'
                case 114: return "\r"; // 'r' -> '\r'
                case 120: return ((char)readHexChar(2)).ToString(); // 'x'
                case 117: return codePointToString(readCodePoint()); // 'u'
                case 116: return "\t"; // 't' -> '\t'
                case 98: return "\b"; // 'b' -> '\b'
                case 118: return "\u000b"; // 'v' -> '\u000b'
                case 102: return "\f"; // 'f' -> '\f'
                case 13:
                    if (input[pos] == 10) ++pos; // '\r\n'
                    goto case 10;
                case 10: // ' \n'
                    lineStart = pos;
                    ++curLine;
                    return "";
                default:
                    if (ch >= 48 && ch <= 55)
                    {
                        var octalStr = new Regex("^[0-7]+").Match(input.Substring(pos - 1, Math.Min(3, input.Length - pos + 1))).Groups[0].Value;
                        var octal = parseInt(octalStr, 8);
                        if (octal > 255)
                        {
                            octalStr = octalStr.Substring(0, octalStr.Length - 1);
                            octal = parseInt(octalStr, 8);
                        }
                        if (octalStr != "0" && (strict || inTemplate))
                        {
                            invalidStringToken(pos - 2, "Octal literal in strict mode");
                        }
                        pos += octalStr.Length - 1;
                        return ((char)octal).ToString();
                    }
                    return ch.ToString();
            }
        }

        private static int parseInt(string str, int @base)
        {
            if (@base > 10)
                throw new NotImplementedException();

            const string numbers = "0123456789";
            var number = 0;
            foreach (var c in str)
            {
                number *= @base;
                var index = numbers.IndexOf(c, 0, @base);
                if (index < 0)
                    throw new NotImplementedException();
                number += index;
            }

            return number;
        }

        // Used to read character escape sequences ('\x', '\u', '\U').
        private int readHexChar(int len)
        {
            var codePos = pos;
            var n = readInt(16, len);
            if (!n.HasValue)
            {
                invalidStringToken(codePos, "Bad character escape sequence");
                return 0;
            }
            return n.Value;
        }

        // Read an identifier, and return it as a string. Sets `this.containsEsc`
        // to whether the word contained a '\u' escape.
        //
        // Incrementally adds only escaped chars, adding other chunks as-is
        // as a micro-optimization.
        private string readWord1()
        {
            containsEsc = false;
            var word = "";
            var first = true;
            var chunkStart = pos;
            var astral = Options.ecmaVersion >= 6;
            while (pos < input.Length)
            {
                var ch = fullCharCodeAtPos();
                if (isIdentifierChar(ch, astral))
                {
                    pos += ch <= 0xffff ? 1 : 2;
                }
                else if (ch == 92)
                {
                    // "\"
                    containsEsc = true;
                    word += input.Substring(chunkStart, pos - chunkStart);
                    var escStart = pos;
                    if (input.Get(++pos) != 117) // "u"
                        invalidStringToken(pos, "Expecting Unicode escape sequence \\uXXXX");
                    ++pos;
                    var esc = readCodePoint();
                    if (!(first ? (Func<int, bool, bool>)isIdentifierStart : isIdentifierChar)(esc, astral))
                        invalidStringToken(escStart, "Invalid Unicode escape");
                    word += codePointToString(esc);
                    chunkStart = pos;
                }
                else
                {
                    break;
                }
                first = false;
            }
            return word + input.Substring(chunkStart, pos - chunkStart);
        }

        // Read an identifier or keyword token. Will check for reserved
        // words when necessary.
        private void readWord()
        {
            var word = readWord1();
            var type = TokenType.name;
            if (keywords.IsMatch(word))
            {
                if (containsEsc) raiseRecoverable(start, "Escape sequence in keyword " + word);
                type = TokenInformation.Keywords[word];
            }
            finishToken(type, word);
        }
    }
}