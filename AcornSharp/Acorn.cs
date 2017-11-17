﻿using System.Collections.Generic;
using AcornSharp.Node;
using JetBrains.Annotations;

namespace AcornSharp
{
    public static class Acorn
    {
        public const string VERSION = "5.2.1";

        // The main exported interface (under `self.acorn` when in the
        // browser) is a `parse` function that takes a code string and
        // returns an abstract syntax tree as specified by [Mozilla parser
        // API][api].
        //
        // [api]: https://developer.mozilla.org/en-US/docs/SpiderMonkey/Parser_API
        [NotNull]
        public static ProgramNode Parse(string input, [CanBeNull] Options options = null)
        {
            return new Parser(options, input).Parse();
        }

        // This function tries to parse a single expression at a given
        // offset in a string. Useful for parsing mixed-language formats
        // that embed JavaScript expressions.
        public static ExpressionNode ParseExpressionAt(string input, int pos, [CanBeNull] Options options = null)
        {
            var p = new Parser(options, input, pos);
            p.nextToken();
            return p.ParseExpression();
        }

        // Acorn is organized as a tokenizer and a recursive-descent parser.
        // The `tokenizer` export provides an interface to the tokenizer.
        [NotNull]
        public static IEnumerable<Token> Tokeniser(string input, [CanBeNull] Options options = null)
        {
            return new Parser(options, input);
        }
    }
}
