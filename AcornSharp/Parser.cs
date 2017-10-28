﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AcornSharp
{
    public sealed partial class Parser
    {
        private sealed class Label
        {
            public string name;
            public string kind;
            public int statementStart;
        }

        internal string sourceFile;
        private string input;
        private bool containsEsc;
        private Regex keywords;
        private Regex reservedWords;
        private Regex reservedWordsStrict;
        private Regex reservedWordsStrictBind;
        private int pos;
        private int lineStart;
        private int curLine;
        private int end;
        private Stack<Scope> scopeStack;
        private List<Label> labels;
        private int start;
        private int lastTokStart;
        private int lastTokEnd;
        private int awaitPos;
        private int yieldPos;
        private int potentialArrowAt;
        private bool inAsync;
        private bool inGenerator;
        private bool inFunction;
        private TokenType type;
        private object value;
        private Position startLoc;
        private Position endLoc;
        private Position lastTokStartLoc;
        private Position lastTokEndLoc;
        private bool exprAllowed;
        private List<TokContext> context;
        private bool inModule;
        private bool strict;
        private int shorthandAssign;
        private int trailingComma;
        private int parenthesizedAssign;
        private int parenthesizedBind;
        private bool inTemplateElement;

        public Parser(Options options, string input, int? startPos = null)
        {
            Options = options = Options.getOptions(options);
            sourceFile = options.sourceFile;
            keywords = keywordRegexp(options.ecmaVersion >= 6 ? ecmascript6Keywords : ecmascript5Keywords);
            string reserved = null;
            if (options.allowReserved == null || options.allowReserved is bool && (bool)options.allowReserved == false)
            {
                if (options.ecmaVersion < 3)
                {
                }
                else if (options.ecmaVersion < 5)
                    reserved = ecmascript3ReservedWords;
                else if (options.ecmaVersion < 6)
                    reserved = ecmascript5ReservedWords;
                else
                    reserved = ecmascript6ReservedWords;

                if (options.sourceType == "module") reserved += " await";
            }
            reservedWords = keywordRegexp(reserved);
            var reservedStrict = (reserved != null ? reserved + " " : "") + strictReservedWords;
            reservedWordsStrict = keywordRegexp(reservedStrict);
            reservedWordsStrictBind = keywordRegexp(reservedStrict + " " + strictBindReservedWords);
            this.input = input;

            // Used to signal to callers of `readWord1` whether the word
            // contained any escape sequences. This is needed because words with
            // escape sequences must not be interpreted as keywords.
            containsEsc = false;

            // Set up token state

            // The current position of the tokenizer in the input.
            if (startPos.HasValue)
            {
                pos = startPos.Value;
                lineStart = this.input.LastIndexOf('\n', startPos.Value - 1) + 1;
//                curLine = this.input.slice(0, lineStart).split(lineBreak).length;
                throw new NotImplementedException();
            }
            else
            {
                pos = lineStart = 0;
                curLine = 1;
            }

            // Properties of the current token:
            // Its type
            type = TokenType.eof;
            // For tokens that include more information than their type, the value
            value = null;
            // Its start and end offset
            start = end = pos;
            // And, if locations are used, the {line, column} object
            // corresponding to those offsets
            startLoc = endLoc = curPosition();

            // Position information for the previous token
            lastTokEndLoc = lastTokStartLoc = null;
            lastTokStart = lastTokEnd = pos;

            // The context stack is used to superficially track syntactic
            // context to predict whether a regular expression is allowed in a
            // given position.
            context = initialContext();
            exprAllowed = true;

            // Figure out if it's a module code.
            inModule = options.sourceType == "module";
            strict = inModule || strictDirective(pos);

            // Used to signify the start of a potential arrow function
            potentialArrowAt = -1;

            // Flags to track whether we are in a function, a generator, an async function.
            inFunction = inGenerator = inAsync = false;
            // Positions to delayed-check that yield/await does not exist in default parameters.
            yieldPos = awaitPos = 0;
            // Labels in scope.
            labels = new List<Label>();

            // If enabled, skip leading hashbang line.
            if (pos == 0 && options.allowHashBang && this.input.Substring(0, 2) == "#!")
                skipLineComment(2);

            // Scope tracking for duplicate variable names (see scope.js)
            scopeStack = new Stack<Scope>();
            enterFunctionScope();
        }

        public Node Parse()
        {
            var node = Options.program ?? startNode();
            nextToken();
            return parseTopLevel(node);
        }

        private static Regex keywordRegexp(string words)
        {
            return new Regex("^(?:" + string.Join('|', words.Split(' ')) + ")$");
        }

        public Options Options { get; }
    }
}