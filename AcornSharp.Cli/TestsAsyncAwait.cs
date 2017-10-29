﻿using System.Collections.Generic;

namespace AcornSharp.Cli
{
    internal static partial class Program
    {
        private static void TestsAsyncAwait()
        {
            //-----------------------------------------------------------------------------
            // Async Function Declarations

            // async == false
            Test("function foo() { }", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 18,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.FunctionDeclaration,
                        start = 0,
                        end = 18,
                        id = new IdentifierNode(new SourceLocation(new Position(1, 9, 9), new Position(1, 12, 12)), "foo"),
                        generator = false,
                        bexpression = false,
                        async = false,
                        @params = new List<Node>(),
                        fbody = new Node
                        {
                            type = NodeType.BlockStatement,
                            start = 15,
                            end = 18,
                            body = new List<Node>()
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            // async == true
            Test("async function foo() { }", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 24,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.FunctionDeclaration,
                        start = 0,
                        end = 24,
                        id = new IdentifierNode(new SourceLocation(new Position(1, 15, 15), new Position(1, 18, 18)), "foo"),
                        generator = false,
                        bexpression = false,
                        async = true,
                        @params = new List<Node>(),
                        fbody = new Node
                        {
                            type = NodeType.BlockStatement,
                            start = 21,
                            end = 24,
                            body = new List<Node>()
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            // a reference and a normal function declaration if there is a linebreak between 'async' and 'function'.
            Test("async\nfunction foo() { }", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 24,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExpressionStatement,
                        start = 0,
                        end = 5,
                        expression = new IdentifierNode(new SourceLocation(new Position(1, 0, 0), new Position(1, 5, 5)), "async")
                    },
                    new Node
                    {
                        type = NodeType.FunctionDeclaration,
                        start = 6,
                        end = 24,
                        id = new IdentifierNode(new SourceLocation(new Position(2, 9, 15), new Position(2, 12, 18)), "foo"),
                        generator = false,
                        bexpression = false,
                        async = false,
                        @params = new List<Node>(),
                        fbody = new Node
                        {
                            type = NodeType.BlockStatement,
                            start = 21,
                            end = 24,
                            body = new List<Node>()
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            // export
            Test("export async function foo() { }", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 31,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExportNamedDeclaration,
                        start = 0,
                        end = 31,
                        declaration = new Node
                        {
                            type = NodeType.FunctionDeclaration,
                            start = 7,
                            end = 31,
                            id = new IdentifierNode(new SourceLocation(new Position(1, 22, 22), new Position(1, 25, 25)), "foo"),
                            generator = false,
                            bexpression = false,
                            async = true,
                            @params = new List<Node>(),
                            fbody = new Node
                            {
                                type = NodeType.BlockStatement,
                                start = 28,
                                end = 31,
                                body = new List<Node>()
                            }
                        },
                        specifiers = new List<Node>(),
                        source = null
                    }
                },
                sourceType = "module"
            }, new Options {ecmaVersion = 8, sourceType = "module"});

            // export default
            Test("export default async function() { }", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 35,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExportDefaultDeclaration,
                        start = 0,
                        end = 35,
                        declaration = new Node
                        {
                            type = NodeType.FunctionDeclaration,
                            start = 15,
                            end = 35,
                            id = null,
                            generator = false,
                            bexpression = false,
                            async = true,
                            @params = new List<Node>(),
                            fbody = new Node
                            {
                                type = NodeType.BlockStatement,
                                start = 32,
                                end = 35,
                                body = new List<Node>()
                            }
                        }
                    }
                },
                sourceType = "module"
            }, new Options {ecmaVersion = 8, sourceType = "module"});

            // cannot combine with generators
            testFail("async function* foo() { }", "Unexpected token (1:14)", new Options {ecmaVersion = 8});

            // 'await' is valid as function names.
            Test("async function await() { }", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 26,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.FunctionDeclaration,
                        start = 0,
                        end = 26,
                        id = new IdentifierNode(new SourceLocation(new Position(1, 15, 15), new Position(1, 20, 20)), "await"),
                        generator = false,
                        bexpression = false,
                        async = true,
                        @params = new List<Node>(),
                        fbody = new Node
                        {
                            type = NodeType.BlockStatement,
                            start = 23,
                            end = 26,
                            body = new List<Node>()
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            // cannot use 'await' inside async functions.
            testFail("async function wrap() {\nasync function await() { }\n}", "Can not use 'await' as identifier inside an async function (2:15)", new Options {ecmaVersion = 8});
            testFail("async function foo(await) { }", "Can not use 'await' as identifier inside an async function (1:19)", new Options {ecmaVersion = 8});
            testFail("async function foo() { return {await} }", "Can not use 'await' as identifier inside an async function (1:31)", new Options {ecmaVersion = 8});

            //-----------------------------------------------------------------------------
            // Async Function Expressions

            // async == false
            Test("(function foo() { })", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 20,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExpressionStatement,
                        start = 0,
                        end = 20,
                        expression = new Node
                        {
                            type = NodeType.FunctionExpression,
                            start = 1,
                            end = 19,
                            id = new IdentifierNode(new SourceLocation(new Position(1, 10, 10), new Position(1, 13, 13)), "foo"),
                            generator = false,
                            bexpression = false,
                            async = false,
                            @params = new List<Node>(),
                            fbody = new Node
                            {
                                type = NodeType.BlockStatement,
                                start = 16,
                                end = 19,
                                body = new List<Node>()
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            // async == true
            Test("(async function foo() { })", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 26,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExpressionStatement,
                        start = 0,
                        end = 26,
                        expression = new Node
                        {
                            type = NodeType.FunctionExpression,
                            start = 1,
                            end = 25,
                            id = new IdentifierNode(new SourceLocation(new Position(1, 16, 16), new Position(1, 19, 19)), "foo"),
                            generator = false,
                            bexpression = false,
                            async = true,
                            @params = new List<Node>(),
                            fbody = new Node
                            {
                                type = NodeType.BlockStatement,
                                start = 22,
                                end = 25,
                                body = new List<Node>()
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            // cannot insert a linebreak to between 'async' and 'function'.
            testFail("(async\nfunction foo() { })", "Unexpected token (2:0)", new Options {ecmaVersion = 8});

            // cannot combine with generators.
            testFail("(async function* foo() { })", "Unexpected token (1:15)", new Options {ecmaVersion = 8});

            // export default
            Test("export default (async function() { })", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 37,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExportDefaultDeclaration,
                        start = 0,
                        end = 37,
                        declaration = new Node
                        {
                            type = NodeType.FunctionExpression,
                            start = 16,
                            end = 36,
                            id = null,
                            generator = false,
                            bexpression = false,
                            async = true,
                            @params = new List<Node>(),
                            fbody = new Node
                            {
                                type = NodeType.BlockStatement,
                                start = 33,
                                end = 36,
                                body = new List<Node>()
                            }
                        }
                    }
                },
                sourceType = "module"
            }, new Options {ecmaVersion = 8, sourceType = "module"});

            // cannot use 'await' inside async functions.
            testFail("(async function await() { })", "Can not use 'await' as identifier inside an async function (1:16)", new Options {ecmaVersion = 8});
            testFail("(async function foo(await) { })", "Can not use 'await' as identifier inside an async function (1:20)", new Options {ecmaVersion = 8});
            testFail("(async function foo() { return {await} })", "Can not use 'await' as identifier inside an async function (1:32)", new Options {ecmaVersion = 8});

            //-----------------------------------------------------------------------------
            // Async Arrow Function Expressions

            // async == false
            Test("a => a", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 6,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExpressionStatement,
                        start = 0,
                        end = 6,
                        expression = new Node
                        {
                            type = NodeType.ArrowFunctionExpression,
                            start = 0,
                            end = 6,
                            id = null,
                            generator = false,
                            bexpression = true,
                            async = false,
                            @params = new List<Node>
                            {
                                new IdentifierNode(new SourceLocation(new Position(1, 0, 0), new Position(1, 1, 1)), "a")
                            },
                            fbody = new IdentifierNode(new SourceLocation(new Position(1, 5, 5), new Position(1, 6, 6)), "a")
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            Test("(a) => a", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 8,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExpressionStatement,
                        start = 0,
                        end = 8,
                        expression = new Node
                        {
                            type = NodeType.ArrowFunctionExpression,
                            start = 0,
                            end = 8,
                            id = null,
                            generator = false,
                            bexpression = true,
                            async = false,
                            @params = new List<Node>
                            {
                                new IdentifierNode(new SourceLocation(new Position(1, 1, 1), new Position(1, 2, 2)), "a")
                            },
                            fbody = new IdentifierNode(new SourceLocation(new Position(1, 7, 7), new Position(1, 8, 8)), "a")
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            // async == true
            Test("async a => a", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 12,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExpressionStatement,
                        start = 0,
                        end = 12,
                        expression = new Node
                        {
                            type = NodeType.ArrowFunctionExpression,
                            start = 0,
                            end = 12,
                            id = null,
                            generator = false,
                            bexpression = true,
                            async = true,
                            @params = new List<Node>
                            {
                                new IdentifierNode(new SourceLocation(new Position(1, 6, 6), new Position(1, 7, 7)), "a")
                            },
                            fbody = new IdentifierNode(new SourceLocation(new Position(1, 11, 11), new Position(1, 12, 12)), "a")
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            Test("async () => a", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 13,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExpressionStatement,
                        start = 0,
                        end = 13,
                        expression = new Node
                        {
                            type = NodeType.ArrowFunctionExpression,
                            start = 0,
                            end = 13,
                            id = null,
                            generator = false,
                            bexpression = true,
                            async = true,
                            @params = new List<Node>(),
                            fbody = new IdentifierNode(new SourceLocation(new Position(1, 12, 12), new Position(1, 13, 13)), "a")
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            Test("async (a, b) => a", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 17,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExpressionStatement,
                        start = 0,
                        end = 17,
                        expression = new Node
                        {
                            type = NodeType.ArrowFunctionExpression,
                            start = 0,
                            end = 17,
                            id = null,
                            generator = false,
                            bexpression = true,
                            async = true,
                            @params = new List<Node>
                            {
                                new IdentifierNode(new SourceLocation(new Position(1, 7, 7), new Position(1, 8, 8)), "a"),
                                new IdentifierNode(new SourceLocation(new Position(1, 10, 10), new Position(1, 11, 11)), "b")
                            },
                            fbody = new IdentifierNode(new SourceLocation(new Position(1, 16, 16), new Position(1, 17, 17)), "a")
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            // OK even if it's an invalid syntax in the case `=>` didn't exist.
            Test("async ({a = b}) => a", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 20,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExpressionStatement,
                        start = 0,
                        end = 20,
                        expression = new Node
                        {
                            type = NodeType.ArrowFunctionExpression,
                            start = 0,
                            end = 20,
                            id = null,
                            generator = false,
                            bexpression = true,
                            async = true,
                            @params = new List<Node>
                            {
                                new Node
                                {
                                    type = NodeType.ObjectPattern,
                                    start = 7,
                                    end = 14,
                                    properties = new List<Node>
                                    {
                                        new Node
                                        {
                                            type = NodeType.Property,
                                            start = 8,
                                            end = 13,
                                            method = false,
                                            shorthand = true,
                                            computed = false,
                                            key = new IdentifierNode(new SourceLocation(new Position(1, 8, 8), new Position(1, 9, 9)), "a"),
                                            kind = "init",
                                            value = new Node
                                            {
                                                type = NodeType.AssignmentPattern,
                                                start = 8,
                                                end = 13,
                                                left = new IdentifierNode(new SourceLocation(new Position(1, 8, 8), new Position(1, 9, 9)), "a"),
                                                right = new IdentifierNode(new SourceLocation(new Position(1, 12, 12), new Position(1, 13, 13)), "b")
                                            }
                                        }
                                    }
                                }
                            },
                            fbody = new IdentifierNode(new SourceLocation(new Position(1, 19, 19), new Position(1, 20, 20)), "a")
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            // syntax error if `=>` didn't exist.
            testFail("async ({a = b})", "Shorthand property assignments are valid only in destructuring patterns (1:10)", new Options {ecmaVersion = 8});

            // AssignmentPattern/AssignmentExpression
            Test("async ({a: b = c}) => a", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 23,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExpressionStatement,
                        start = 0,
                        end = 23,
                        expression = new Node
                        {
                            type = NodeType.ArrowFunctionExpression,
                            start = 0,
                            end = 23,
                            id = null,
                            generator = false,
                            bexpression = true,
                            async = true,
                            @params = new List<Node>
                            {
                                new Node
                                {
                                    type = NodeType.ObjectPattern,
                                    start = 7,
                                    end = 17,
                                    properties = new List<Node>
                                    {
                                        new Node
                                        {
                                            type = NodeType.Property,
                                            start = 8,
                                            end = 16,
                                            method = false,
                                            shorthand = false,
                                            computed = false,
                                            key = new IdentifierNode(new SourceLocation(new Position(1, 8, 8), new Position(1, 9, 9)), "a"),
                                            value = new Node
                                            {
                                                type = NodeType.AssignmentPattern,
                                                start = 11,
                                                end = 16,
                                                left = new IdentifierNode(new SourceLocation(new Position(1, 11, 11), new Position(1, 12, 12)), "b"),
                                                right = new IdentifierNode(new SourceLocation(new Position(1, 15, 15), new Position(1, 16, 16)), "c")
                                            },
                                            kind = "init"
                                        }
                                    }
                                }
                            },
                            fbody = new IdentifierNode(new SourceLocation(new Position(1, 22, 22), new Position(1, 23, 23)), "a")
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            Test("async ({a: b = c})", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 18,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExpressionStatement,
                        start = 0,
                        end = 18,
                        expression = new Node
                        {
                            type = NodeType.CallExpression,
                            start = 0,
                            end = 18,
                            callee = new IdentifierNode(new SourceLocation(new Position(1, 0, 0), new Position(1, 5, 5)), "async"),
                            arguments = new List<Node>
                            {
                                new Node
                                {
                                    type = NodeType.ObjectExpression,
                                    start = 7,
                                    end = 17,
                                    properties = new List<Node>
                                    {
                                        new Node
                                        {
                                            type = NodeType.Property,
                                            start = 8,
                                            end = 16,
                                            method = false,
                                            shorthand = false,
                                            computed = false,
                                            key = new IdentifierNode(new SourceLocation(new Position(1, 8, 8), new Position(1, 9, 9)), "a"),
                                            value = new Node
                                            {
                                                type = NodeType.AssignmentExpression,
                                                start = 11,
                                                end = 16,
                                                @operator = "=",
                                                left = new IdentifierNode(new SourceLocation(new Position(1, 11, 11), new Position(1, 12, 12)), "b"),
                                                right = new IdentifierNode(new SourceLocation(new Position(1, 15, 15), new Position(1, 16, 16)), "c")
                                            },
                                            kind = "init"
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            // a reference and a normal arrow function if there is a linebreak between 'async' and the 1st parameter.
            Test("async\na => a", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 12,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExpressionStatement,
                        start = 0,
                        end = 5,
                        expression = new IdentifierNode(new SourceLocation(new Position(1, 0, 0), new Position(1, 5, 5)), "async")
                    },
                    new Node
                    {
                        type = NodeType.ExpressionStatement,
                        start = 6,
                        end = 12,
                        expression = new Node
                        {
                            type = NodeType.ArrowFunctionExpression,
                            start = 6,
                            end = 12,
                            id = null,
                            generator = false,
                            bexpression = true,
                            async = false,
                            @params = new List<Node>
                            {
                                new IdentifierNode(new SourceLocation(new Position(2, 0, 6), new Position(2, 1, 7)), "a")
                            },
                            fbody = new IdentifierNode(new SourceLocation(new Position(2, 5, 11), new Position(2, 6, 12)), "a")
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            // 'async()' call expression and invalid '=>' token.
            testFail("async\n() => a", "Unexpected token (2:3)", new Options {ecmaVersion = 8});

            // cannot insert a linebreak before '=>'.
            testFail("async a\n=> a", "Unexpected token (2:0)", new Options {ecmaVersion = 8});
            testFail("async ()\n=> a", "Unexpected token (2:0)", new Options {ecmaVersion = 8});

            // a call expression with 'await' reference.
            Test("async (await)", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 13,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExpressionStatement,
                        start = 0,
                        end = 13,
                        expression = new Node
                        {
                            type = NodeType.CallExpression,
                            start = 0,
                            end = 13,
                            callee = new IdentifierNode(new SourceLocation(new Position(1, 0, 0), new Position(1, 5, 5)), "async"),
                            arguments = new List<Node>
                            {
                                new IdentifierNode(new SourceLocation(new Position(1, 7, 7), new Position(1, 12, 12)), "await")
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            // cannot use 'await' inside async functions.
            testFail("async await => 1", "Can not use 'await' as identifier inside an async function (1:6)", new Options {ecmaVersion = 8});
            testFail("async (await) => 1", "Can not use 'await' as identifier inside an async function (1:7)", new Options {ecmaVersion = 8});
            testFail("async ({await}) => 1", "Can not use 'await' as identifier inside an async function (1:8)", new Options {ecmaVersion = 8});
            testFail("async ({a: await}) => 1", "Can not use 'await' as identifier inside an async function (1:11)", new Options {ecmaVersion = 8});
            testFail("async ([await]) => 1", "Can not use 'await' as identifier inside an async function (1:8)", new Options {ecmaVersion = 8});

            // can use 'yield' identifier outside generators.
            Test("async yield => 1", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 16,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExpressionStatement,
                        start = 0,
                        end = 16,
                        expression = new Node
                        {
                            type = NodeType.ArrowFunctionExpression,
                            start = 0,
                            end = 16,
                            id = null,
                            generator = false,
                            bexpression = true,
                            async = true,
                            @params = new List<Node>
                            {
                                new IdentifierNode(new SourceLocation(new Position(1, 6, 6), new Position(1, 11, 11)), "yield")
                            },
                            fbody = new Node
                            {
                                type = NodeType.Literal,
                                start = 15,
                                end = 16,
                                value = 1,
                                raw = "1"
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            //-----------------------------------------------------------------------------
            // Async Methods (object)

            // async == false
            Test("({foo() { }})", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 13,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExpressionStatement,
                        start = 0,
                        end = 13,
                        expression = new Node
                        {
                            type = NodeType.ObjectExpression,
                            start = 1,
                            end = 12,
                            properties = new List<Node>
                            {
                                new Node
                                {
                                    type = NodeType.Property,
                                    start = 2,
                                    end = 11,
                                    method = true,
                                    shorthand = false,
                                    computed = false,
                                    key = new IdentifierNode(new SourceLocation(new Position(1, 2, 2), new Position(1, 5, 5)), "foo"),
                                    kind = "init",
                                    value = new Node
                                    {
                                        type = NodeType.FunctionExpression,
                                        start = 5,
                                        end = 11,
                                        id = null,
                                        generator = false,
                                        bexpression = false,
                                        async = false,
                                        @params = new List<Node>(),
                                        fbody = new Node
                                        {
                                            type = NodeType.BlockStatement,
                                            start = 8,
                                            end = 11,
                                            body = new List<Node>()
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            // async == true
            Test("({async foo() { }})", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 19,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExpressionStatement,
                        start = 0,
                        end = 19,
                        expression = new Node
                        {
                            type = NodeType.ObjectExpression,
                            start = 1,
                            end = 18,
                            properties = new List<Node>
                            {
                                new Node
                                {
                                    type = NodeType.Property,
                                    start = 2,
                                    end = 17,
                                    method = true,
                                    shorthand = false,
                                    computed = false,
                                    key = new IdentifierNode(new SourceLocation(new Position(1, 8, 8), new Position(1, 11, 11)), "foo"),
                                    kind = "init",
                                    value = new Node
                                    {
                                        type = NodeType.FunctionExpression,
                                        start = 11,
                                        end = 17,
                                        id = null,
                                        generator = false,
                                        bexpression = false,
                                        async = true,
                                        @params = new List<Node>(),
                                        fbody = new Node
                                        {
                                            type = NodeType.BlockStatement,
                                            start = 14,
                                            end = 17,
                                            body = new List<Node>()
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            // OK with 'async' as a method name
            Test("({async() { }})", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 15,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExpressionStatement,
                        start = 0,
                        end = 15,
                        expression = new Node
                        {
                            type = NodeType.ObjectExpression,
                            start = 1,
                            end = 14,
                            properties = new List<Node>
                            {
                                new Node
                                {
                                    type = NodeType.Property,
                                    start = 2,
                                    end = 13,
                                    method = true,
                                    shorthand = false,
                                    computed = false,
                                    key = new IdentifierNode(new SourceLocation(new Position(1, 2, 2), new Position(1, 7, 7)), "async"),
                                    kind = "init",
                                    value = new Node
                                    {
                                        type = NodeType.FunctionExpression,
                                        start = 7,
                                        end = 13,
                                        id = null,
                                        generator = false,
                                        bexpression = false,
                                        async = false,
                                        @params = new List<Node>(),
                                        fbody = new Node
                                        {
                                            type = NodeType.BlockStatement,
                                            start = 10,
                                            end = 13,
                                            body = new List<Node>()
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            // invalid syntax if there is a linebreak after 'async'.
            testFail("({async\nfoo() { }})", "Unexpected token (2:0)", new Options {ecmaVersion = 8});

            // cannot combine with getters/setters/generators.
            testFail("({async get foo() { }})", "Unexpected token (1:12)", new Options {ecmaVersion = 8});
            testFail("({async set foo(value) { }})", "Unexpected token (1:12)", new Options {ecmaVersion = 8});
            testFail("({async* foo() { }})", "Unexpected token (1:7)", new Options {ecmaVersion = 8});

            // 'await' is valid as function names.
            Test("({async await() { }})", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 21,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExpressionStatement,
                        start = 0,
                        end = 21,
                        expression = new Node
                        {
                            type = NodeType.ObjectExpression,
                            start = 1,
                            end = 20,
                            properties = new List<Node>
                            {
                                new Node
                                {
                                    type = NodeType.Property,
                                    start = 2,
                                    end = 19,
                                    method = true,
                                    shorthand = false,
                                    computed = false,
                                    key = new IdentifierNode(new SourceLocation(new Position(1, 8, 8), new Position(1, 13, 13)), "await"),
                                    kind = "init",
                                    value = new Node
                                    {
                                        type = NodeType.FunctionExpression,
                                        start = 13,
                                        end = 19,
                                        id = null,
                                        generator = false,
                                        bexpression = false,
                                        async = true,
                                        @params = new List<Node>(),
                                        fbody = new Node
                                        {
                                            type = NodeType.BlockStatement,
                                            start = 16,
                                            end = 19,
                                            body = new List<Node>()
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            // cannot use 'await' inside async functions.
            Test("async function wrap() {\n({async await() { }})\n}", new Node { }, new Options {ecmaVersion = 8});
            testFail("({async foo() { var await }})", "Can not use 'await' as identifier inside an async function (1:20)", new Options {ecmaVersion = 8});
            testFail("({async foo(await) { }})", "Can not use 'await' as identifier inside an async function (1:12)", new Options {ecmaVersion = 8});
            testFail("({async foo() { return {await} }})", "Can not use 'await' as identifier inside an async function (1:24)", new Options {ecmaVersion = 8});

            // invalid syntax 'async foo: 1'
            testFail("({async foo: 1})", "Unexpected token (1:11)", new Options {ecmaVersion = 8});

            //-----------------------------------------------------------------------------
            // Async Methods (class)

            // async == false
            Test("class A {foo() { }}", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 19,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ClassDeclaration,
                        start = 0,
                        end = 19,
                        id = new IdentifierNode(new SourceLocation(new Position(1, 6, 6), new Position(1, 7, 7)), "A"),
                        superClass = null,
                        fbody = new Node
                        {
                            type = NodeType.ClassBody,
                            start = 8,
                            end = 19,
                            body = new List<Node>
                            {
                                new Node
                                {
                                    type = NodeType.MethodDefinition,
                                    start = 9,
                                    end = 18,
                                    computed = false,
                                    key = new IdentifierNode(new SourceLocation(new Position(1, 9, 9), new Position(1, 12, 12)), "foo"),
                                    @static = false,
                                    kind = "method",
                                    value = new Node
                                    {
                                        type = NodeType.FunctionExpression,
                                        start = 12,
                                        end = 18,
                                        id = null,
                                        generator = false,
                                        bexpression = false,
                                        async = false,
                                        @params = new List<Node>(),
                                        fbody = new Node
                                        {
                                            type = NodeType.BlockStatement,
                                            start = 15,
                                            end = 18,
                                            body = new List<Node>()
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            // async == true
            Test("class A {async foo() { }}", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 25,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ClassDeclaration,
                        start = 0,
                        end = 25,
                        id = new IdentifierNode(new SourceLocation(new Position(1, 6, 6), new Position(1, 7, 7)), "A"),
                        superClass = null,
                        fbody = new Node
                        {
                            type = NodeType.ClassBody,
                            start = 8,
                            end = 25,
                            body = new List<Node>
                            {
                                new Node
                                {
                                    type = NodeType.MethodDefinition,
                                    start = 9,
                                    end = 24,
                                    computed = false,
                                    key = new IdentifierNode(new SourceLocation(new Position(1, 15, 15), new Position(1, 18, 18)), "foo"),
                                    @static = false,
                                    kind = "method",
                                    value = new Node
                                    {
                                        type = NodeType.FunctionExpression,
                                        start = 18,
                                        end = 24,
                                        id = null,
                                        generator = false,
                                        bexpression = false,
                                        async = true,
                                        @params = new List<Node>(),
                                        fbody = new Node
                                        {
                                            type = NodeType.BlockStatement,
                                            start = 21,
                                            end = 24,
                                            body = new List<Node>()
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            Test("class A {static async foo() { }}", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 32,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ClassDeclaration,
                        start = 0,
                        end = 32,
                        id = new IdentifierNode(new SourceLocation(new Position(1, 6, 6), new Position(1, 7, 7)), "A"),
                        superClass = null,
                        fbody = new Node
                        {
                            type = NodeType.ClassBody,
                            start = 8,
                            end = 32,
                            body = new List<Node>
                            {
                                new Node
                                {
                                    type = NodeType.MethodDefinition,
                                    start = 9,
                                    end = 31,
                                    computed = false,
                                    key = new IdentifierNode(new SourceLocation(new Position(1, 22, 22), new Position(1, 25, 25)), "foo"),
                                    @static = true,
                                    kind = "method",
                                    value = new Node
                                    {
                                        type = NodeType.FunctionExpression,
                                        start = 25,
                                        end = 31,
                                        id = null,
                                        generator = false,
                                        bexpression = false,
                                        async = true,
                                        @params = new List<Node>(),
                                        fbody = new Node
                                        {
                                            type = NodeType.BlockStatement,
                                            start = 28,
                                            end = 31,
                                            body = new List<Node>()
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            // OK 'async' as a method name.
            Test("class A {async() { }}", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 21,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ClassDeclaration,
                        start = 0,
                        end = 21,
                        id = new IdentifierNode(new SourceLocation(new Position(1, 6, 6), new Position(1, 7, 7)), "A"),
                        superClass = null,
                        fbody = new Node
                        {
                            type = NodeType.ClassBody,
                            start = 8,
                            end = 21,
                            body = new List<Node>
                            {
                                new Node
                                {
                                    type = NodeType.MethodDefinition,
                                    start = 9,
                                    end = 20,
                                    computed = false,
                                    key = new IdentifierNode(new SourceLocation(new Position(1, 9, 9), new Position(1, 14, 14)), "async"),
                                    @static = false,
                                    kind = "method",
                                    value = new Node
                                    {
                                        type = NodeType.FunctionExpression,
                                        start = 14,
                                        end = 20,
                                        id = null,
                                        generator = false,
                                        bexpression = false,
                                        async = false,
                                        @params = new List<Node>(),
                                        fbody = new Node
                                        {
                                            type = NodeType.BlockStatement,
                                            start = 17,
                                            end = 20,
                                            body = new List<Node>()
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            Test("class A {static async() { }}", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 28,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ClassDeclaration,
                        start = 0,
                        end = 28,
                        id = new IdentifierNode(new SourceLocation(new Position(1, 6, 6), new Position(1, 7, 7)), "A"),
                        superClass = null,
                        fbody = new Node
                        {
                            type = NodeType.ClassBody,
                            start = 8,
                            end = 28,
                            body = new List<Node>
                            {
                                new Node
                                {
                                    type = NodeType.MethodDefinition,
                                    start = 9,
                                    end = 27,
                                    computed = false,
                                    key = new IdentifierNode(new SourceLocation(new Position(1, 16, 16), new Position(1, 21, 21)), "async"),
                                    @static = true,
                                    kind = "method",
                                    value = new Node
                                    {
                                        type = NodeType.FunctionExpression,
                                        start = 21,
                                        end = 27,
                                        id = null,
                                        generator = false,
                                        bexpression = false,
                                        async = false,
                                        @params = new List<Node>(),
                                        fbody = new Node
                                        {
                                            type = NodeType.BlockStatement,
                                            start = 24,
                                            end = 27,
                                            body = new List<Node>()
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            Test("class A {*async() { }}", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 22,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ClassDeclaration,
                        start = 0,
                        end = 22,
                        id = new IdentifierNode(new SourceLocation(new Position(1, 6, 6), new Position(1, 7, 7)), "A"),
                        superClass = null,
                        fbody = new Node
                        {
                            type = NodeType.ClassBody,
                            start = 8,
                            end = 22,
                            body = new List<Node>
                            {
                                new Node
                                {
                                    type = NodeType.MethodDefinition,
                                    start = 9,
                                    end = 21,
                                    computed = false,
                                    key = new IdentifierNode(new SourceLocation(new Position(1, 10, 10), new Position(1, 15, 15)), "async"),
                                    @static = false,
                                    kind = "method",
                                    value = new Node
                                    {
                                        type = NodeType.FunctionExpression,
                                        start = 15,
                                        end = 21,
                                        id = null,
                                        generator = true,
                                        bexpression = false,
                                        async = false,
                                        @params = new List<Node>(),
                                        fbody = new Node
                                        {
                                            type = NodeType.BlockStatement,
                                            start = 18,
                                            end = 21,
                                            body = new List<Node>()
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            Test("class A {static* async() { }}", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 29,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ClassDeclaration,
                        start = 0,
                        end = 29,
                        id = new IdentifierNode(new SourceLocation(new Position(1, 6, 6), new Position(1, 7, 7)), "A"),
                        superClass = null,
                        fbody = new Node
                        {
                            type = NodeType.ClassBody,
                            start = 8,
                            end = 29,
                            body = new List<Node>
                            {
                                new Node
                                {
                                    type = NodeType.MethodDefinition,
                                    start = 9,
                                    end = 28,
                                    computed = false,
                                    key = new IdentifierNode(new SourceLocation(new Position(1, 17, 17), new Position(1, 22, 22)), "async"),
                                    @static = true,
                                    kind = "method",
                                    value = new Node
                                    {
                                        type = NodeType.FunctionExpression,
                                        start = 22,
                                        end = 28,
                                        id = null,
                                        generator = true,
                                        bexpression = false,
                                        async = false,
                                        @params = new List<Node>(),
                                        fbody = new Node
                                        {
                                            type = NodeType.BlockStatement,
                                            start = 25,
                                            end = 28,
                                            body = new List<Node>()
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            // invalid syntax if there is a linebreak after 'async'.
            testFail("class A {async\nfoo() { }}", "Unexpected token (2:0)", new Options {ecmaVersion = 8});
            testFail("class A {static async\nfoo() { }}", "Unexpected token (2:0)", new Options {ecmaVersion = 8});

            // cannot combine with constructors/getters/setters/generators.
            testFail("class A {async constructor() { }}", "Constructor can't be an async method (1:15)", new Options {ecmaVersion = 8});
            testFail("class A {async get foo() { }}", "Unexpected token (1:19)", new Options {ecmaVersion = 8});
            testFail("class A {async set foo(value) { }}", "Unexpected token (1:19)", new Options {ecmaVersion = 8});
            testFail("class A {async* foo() { }}", "Unexpected token (1:14)", new Options {ecmaVersion = 8});
            testFail("class A {static async get foo() { }}", "Unexpected token (1:26)", new Options {ecmaVersion = 8});
            testFail("class A {static async set foo(value) { }}", "Unexpected token (1:26)", new Options {ecmaVersion = 8});
            testFail("class A {static async* foo() { }}", "Unexpected token (1:21)", new Options {ecmaVersion = 8});

            // 'await' is valid as function names.
            Test("class A {async await() { }}", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 27,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ClassDeclaration,
                        start = 0,
                        end = 27,
                        id = new IdentifierNode(new SourceLocation(new Position(1, 6, 6), new Position(1, 7, 7)), "A"),
                        superClass = null,
                        fbody = new Node
                        {
                            type = NodeType.ClassBody,
                            start = 8,
                            end = 27,
                            body = new List<Node>
                            {
                                new Node
                                {
                                    type = NodeType.MethodDefinition,
                                    start = 9,
                                    end = 26,
                                    computed = false,
                                    key = new IdentifierNode(new SourceLocation(new Position(1, 15, 15), new Position(1, 20, 20)), "await"),
                                    @static = false,
                                    kind = "method",
                                    value = new Node
                                    {
                                        type = NodeType.FunctionExpression,
                                        start = 20,
                                        end = 26,
                                        id = null,
                                        generator = false,
                                        bexpression = false,
                                        async = true,
                                        @params = new List<Node>(),
                                        fbody = new Node
                                        {
                                            type = NodeType.BlockStatement,
                                            start = 23,
                                            end = 26,
                                            body = new List<Node>()
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            Test("class A {static async await() { }}", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 34,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ClassDeclaration,
                        start = 0,
                        end = 34,
                        id = new IdentifierNode(new SourceLocation(new Position(1, 6, 6), new Position(1, 7, 7)), "A"),
                        superClass = null,
                        fbody = new Node
                        {
                            type = NodeType.ClassBody,
                            start = 8,
                            end = 34,
                            body = new List<Node>
                            {
                                new Node
                                {
                                    type = NodeType.MethodDefinition,
                                    start = 9,
                                    end = 33,
                                    computed = false,
                                    key = new IdentifierNode(new SourceLocation(new Position(1, 22, 22), new Position(1, 27, 27)), "await"),
                                    @static = true,
                                    kind = "method",
                                    value = new Node
                                    {
                                        type = NodeType.FunctionExpression,
                                        start = 27,
                                        end = 33,
                                        id = null,
                                        generator = false,
                                        bexpression = false,
                                        async = true,
                                        @params = new List<Node>(),
                                        fbody = new Node
                                        {
                                            type = NodeType.BlockStatement,
                                            start = 30,
                                            end = 33,
                                            body = new List<Node>()
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            // cannot use 'await' inside async functions.
            Test("async function wrap() {\nclass A {async await() { }}\n}", new Node { }, new Options {ecmaVersion = 8});
            testFail("class A {async foo() { var await }}", "Can not use 'await' as identifier inside an async function (1:27)", new Options {ecmaVersion = 8});
            testFail("class A {async foo(await) { }}", "Can not use 'await' as identifier inside an async function (1:19)", new Options {ecmaVersion = 8});
            testFail("class A {async foo() { return {await} }}", "Can not use 'await' as identifier inside an async function (1:31)", new Options {ecmaVersion = 8});
            //-----------------------------------------------------------------------------
            // Await Expressions

            // 'await' is an identifier in scripts.
            Test("await", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 5,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExpressionStatement,
                        start = 0,
                        end = 5,
                        expression = new IdentifierNode(new SourceLocation(new Position(1, 0, 0), new Position(1, 5, 5)), "await")
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            // 'await' is a keyword in modules.
            testFail("await", "The keyword 'await' is reserved (1:0)", new Options {ecmaVersion = 8, sourceType = "module"});

            // Await expressions is invalid outside of async functions.
            testFail("await a", "Unexpected token (1:6)", new Options {ecmaVersion = 8});
            testFail("await a", "The keyword 'await' is reserved (1:0)", new Options {ecmaVersion = 8, sourceType = "module"});

            // Await expressions in async functions.
            Test("async function foo(a, b) { await a }", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 36,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.FunctionDeclaration,
                        start = 0,
                        end = 36,
                        id =  new IdentifierNode(new SourceLocation(new Position(1, 15, 15), new Position(1, 18, 18)), "foo"),
                        generator = false,
                        bexpression = false,
                        async = true,
                        @params = new List<Node>
                        {
                            new IdentifierNode(new SourceLocation(new Position(1, 19, 19), new Position(1, 20, 20)), "a"),
                            new IdentifierNode(new SourceLocation(new Position(1, 22, 22), new Position(1, 23, 23)), "b")
                        },
                        fbody = new Node
                        {
                            type = NodeType.BlockStatement,
                            start = 25,
                            end = 36,
                            body = new List<Node>
                            {
                                new Node
                                {
                                    type = NodeType.ExpressionStatement,
                                    start = 27,
                                    end = 34,
                                    expression = new Node
                                    {
                                        type = NodeType.AwaitExpression,
                                        start = 27,
                                        end = 34,
                                        argument = new IdentifierNode(new SourceLocation(new Position(1, 33, 33), new Position(1, 34, 34)), "a")
                                    }
                                }
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            Test("(async function foo(a) { await a })", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 35,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExpressionStatement,
                        start = 0,
                        end = 35,
                        expression = new Node
                        {
                            type = NodeType.FunctionExpression,
                            start = 1,
                            end = 34,
                            id =  new IdentifierNode(new SourceLocation(new Position(1, 16, 16), new Position(1, 19, 19)), "foo"),
                            generator = false,
                            bexpression = false,
                            async = true,
                            @params = new List<Node>
                            {
                                new IdentifierNode(new SourceLocation(new Position(1, 20, 20), new Position(1, 21, 21)), "a")
                            },
                            fbody = new Node
                            {
                                type = NodeType.BlockStatement,
                                start = 23,
                                end = 34,
                                body = new List<Node>
                                {
                                    new Node
                                    {
                                        type = NodeType.ExpressionStatement,
                                        start = 25,
                                        end = 32,
                                        expression = new Node
                                        {
                                            type = NodeType.AwaitExpression,
                                            start = 25,
                                            end = 32,
                                            argument = new IdentifierNode(new SourceLocation(new Position(1, 31, 31), new Position(1, 32, 32)), "a")
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            Test("(async (a) => await a)", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 22,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExpressionStatement,
                        start = 0,
                        end = 22,
                        expression = new Node
                        {
                            type = NodeType.ArrowFunctionExpression,
                            start = 1,
                            end = 21,
                            id = null,
                            generator = false,
                            bexpression = true,
                            async = true,
                            @params = new List<Node>
                            {
                                new IdentifierNode(new SourceLocation(new Position(1, 8, 8), new Position(1, 9, 9)), "a")
                            },
                            fbody = new Node
                            {
                                type = NodeType.AwaitExpression,
                                start = 14,
                                end = 21,
                                argument = new IdentifierNode(new SourceLocation(new Position(1, 20, 20), new Position(1, 21, 21)), "a")
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            Test("({async foo(a) { await a }})", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 28,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExpressionStatement,
                        start = 0,
                        end = 28,
                        expression = new Node
                        {
                            type = NodeType.ObjectExpression,
                            start = 1,
                            end = 27,
                            properties = new List<Node>
                            {
                                new Node
                                {
                                    type = NodeType.Property,
                                    start = 2,
                                    end = 26,
                                    method = true,
                                    shorthand = false,
                                    computed = false,
                                    key = new IdentifierNode(new SourceLocation(new Position(1, 8, 8), new Position(1, 11, 11)), "foo"),
                                    kind = "init",
                                    value = new Node
                                    {
                                        type = NodeType.FunctionExpression,
                                        start = 11,
                                        end = 26,
                                        id = null,
                                        generator = false,
                                        bexpression = false,
                                        async = true,
                                        @params = new List<Node>
                                        {
                                            new IdentifierNode(new SourceLocation(new Position(1, 12, 12), new Position(1, 13, 13)), "a")
                                        },
                                        fbody = new Node
                                        {
                                            type = NodeType.BlockStatement,
                                            start = 15,
                                            end = 26,
                                            body = new List<Node>
                                            {
                                                new Node
                                                {
                                                    type = NodeType.ExpressionStatement,
                                                    start = 17,
                                                    end = 24,
                                                    expression = new Node
                                                    {
                                                        type = NodeType.AwaitExpression,
                                                        start = 17,
                                                        end = 24,
                                                        argument = new IdentifierNode(new SourceLocation(new Position(1, 23, 23), new Position(1, 24, 24)), "a")
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            Test("(class {async foo(a) { await a }})", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 34,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExpressionStatement,
                        start = 0,
                        end = 34,
                        expression = new Node
                        {
                            type = NodeType.ClassExpression,
                            start = 1,
                            end = 33,
                            id = null,
                            superClass = null,
                            fbody = new Node
                            {
                                type = NodeType.ClassBody,
                                start = 7,
                                end = 33,
                                body = new List<Node>
                                {
                                    new Node
                                    {
                                        type = NodeType.MethodDefinition,
                                        start = 8,
                                        end = 32,
                                        computed = false,
                                        key = new IdentifierNode(new SourceLocation(new Position(1, 14, 14), new Position(1, 17, 17)), "foo"),
                                        @static = false,
                                        kind = "method",
                                        value = new Node
                                        {
                                            type = NodeType.FunctionExpression,
                                            start = 17,
                                            end = 32,
                                            id = null,
                                            generator = false,
                                            bexpression = false,
                                            async = true,
                                            @params = new List<Node>
                                            {
                                                new IdentifierNode(new SourceLocation(new Position(1, 18, 18), new Position(1, 19, 19)), "a")
                                            },
                                            fbody = new Node
                                            {
                                                type = NodeType.BlockStatement,
                                                start = 21,
                                                end = 32,
                                                body = new List<Node>
                                                {
                                                    new Node
                                                    {
                                                        type = NodeType.ExpressionStatement,
                                                        start = 23,
                                                        end = 30,
                                                        expression = new Node
                                                        {
                                                            type = NodeType.AwaitExpression,
                                                            start = 23,
                                                            end = 30,
                                                            argument = new IdentifierNode(new SourceLocation(new Position(1, 29, 29), new Position(1, 30, 30)), "a")
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            // Await expressions are an unary expression.
            Test("async function foo(a, b) { await a + await b }", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 46,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.FunctionDeclaration,
                        start = 0,
                        end = 46,
                        id =  new IdentifierNode(new SourceLocation(new Position(1, 15, 15), new Position(1, 18, 18)), "foo"),
                        generator = false,
                        bexpression = false,
                        async = true,
                        @params = new List<Node>
                        {
                            new IdentifierNode(new SourceLocation(new Position(1, 19, 19), new Position(1, 20, 20)), "a"),
                            new IdentifierNode(new SourceLocation(new Position(1, 22, 22), new Position(1, 23, 23)), "b")
                        },
                        fbody = new Node
                        {
                            type = NodeType.BlockStatement,
                            start = 25,
                            end = 46,
                            body = new List<Node>
                            {
                                new Node
                                {
                                    type = NodeType.ExpressionStatement,
                                    start = 27,
                                    end = 44,
                                    expression = new Node
                                    {
                                        type = NodeType.BinaryExpression,
                                        start = 27,
                                        end = 44,
                                        left = new Node
                                        {
                                            type = NodeType.AwaitExpression,
                                            start = 27,
                                            end = 34,
                                            argument = new IdentifierNode(new SourceLocation(new Position(1, 33, 33), new Position(1, 34, 34)), "a")
                                        },
                                        @operator = "+",
                                        right = new Node
                                        {
                                            type = NodeType.AwaitExpression,
                                            start = 37,
                                            end = 44,
                                            argument = new IdentifierNode(new SourceLocation(new Position(1, 43, 43), new Position(1, 44, 44)), "b")
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            // 'await + 1' is a binary expression outside of async functions.
            Test("function foo() { await + 1 }", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 28,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.FunctionDeclaration,
                        start = 0,
                        end = 28,
                        id =  new IdentifierNode(new SourceLocation(new Position(1, 9, 9), new Position(1, 12, 12)), "foo"),
                        generator = false,
                        bexpression = false,
                        async = false,
                        @params = new List<Node>(),
                        fbody = new Node
                        {
                            type = NodeType.BlockStatement,
                            start = 15,
                            end = 28,
                            body = new List<Node>
                            {
                                new Node
                                {
                                    type = NodeType.ExpressionStatement,
                                    start = 17,
                                    end = 26,
                                    expression = new Node
                                    {
                                        type = NodeType.BinaryExpression,
                                        start = 17,
                                        end = 26,
                                        left = new IdentifierNode(new SourceLocation(new Position(1, 17, 17), new Position(1, 22, 22)), "await"),
                                        @operator = "+",
                                        right = new Node
                                        {
                                            type = NodeType.Literal,
                                            start = 25,
                                            end = 26,
                                            value = 1,
                                            raw = "1"
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            // 'await + 1' is an await expression in async functions.
            Test("async function foo() { await + 1 }", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 34,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.FunctionDeclaration,
                        start = 0,
                        end = 34,
                        id =  new IdentifierNode(new SourceLocation(new Position(1, 15, 15), new Position(1, 18, 18)), "foo"),
                        generator = false,
                        bexpression = false,
                        async = true,
                        @params = new List<Node>(),
                        fbody = new Node
                        {
                            type = NodeType.BlockStatement,
                            start = 21,
                            end = 34,
                            body = new List<Node>
                            {
                                new Node
                                {
                                    type = NodeType.ExpressionStatement,
                                    start = 23,
                                    end = 32,
                                    expression = new Node
                                    {
                                        type = NodeType.AwaitExpression,
                                        start = 23,
                                        end = 32,
                                        argument = new Node
                                        {
                                            type = NodeType.UnaryExpression,
                                            start = 29,
                                            end = 32,
                                            @operator = "+",
                                            prefix = true,
                                            argument = new Node
                                            {
                                                type = NodeType.Literal,
                                                start = 31,
                                                end = 32,
                                                value = 1,
                                                raw = "1"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            // Await expressions need one argument.
            testFail("async function foo() { await }", "Unexpected token (1:29)", new Options {ecmaVersion = 8});
            testFail("(async function foo() { await })", "Unexpected token (1:30)", new Options {ecmaVersion = 8});
            testFail("async () => await", "Unexpected token (1:17)", new Options {ecmaVersion = 8});
            testFail("({async foo() { await }})", "Unexpected token (1:22)", new Options {ecmaVersion = 8});
            testFail("(class {async foo() { await }})", "Unexpected token (1:28)", new Options {ecmaVersion = 8});

            // Forbid await expressions in default parameters:
            testFail("async function foo(a = await b) {}", "Await expression cannot be a default value (1:23)", new Options {ecmaVersion = 8});
            testFail("(async function foo(a = await b) {})", "Await expression cannot be a default value (1:24)", new Options {ecmaVersion = 8});
            testFail("async (a = await b) => {}", "Unexpected token (1:17)", new Options {ecmaVersion = 8});
            testFail("async function wrapper() {\nasync (a = await b) => {}\n}", "Await expression cannot be a default value (2:11)", new Options {ecmaVersion = 8});
            testFail("({async foo(a = await b) {}})", "Await expression cannot be a default value (1:16)", new Options {ecmaVersion = 8});
            testFail("(class {async foo(a = await b) {}})", "Await expression cannot be a default value (1:22)", new Options {ecmaVersion = 8});
            testFail("async function foo(a = class extends (await b) {}) {}", "Await expression cannot be a default value (1:38)", new Options {ecmaVersion = 8});

            // Allow await expressions inside functions in default parameters:
            Test("async function foo(a = async function foo() { await b }) {}", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 59,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.FunctionDeclaration,
                        start = 0,
                        end = 59,
                        id =  new IdentifierNode(new SourceLocation(new Position(1, 15, 15), new Position(1, 18, 18)), "foo"),
                        generator = false,
                        bexpression = false,
                        async = true,
                        @params = new List<Node>
                        {
                            new Node
                            {
                                type = NodeType.AssignmentPattern,
                                start = 19,
                                end = 55,
                                left = new IdentifierNode(new SourceLocation(new Position(1, 19, 19), new Position(1, 20, 20)), "a"),
                                right = new Node
                                {
                                    type = NodeType.FunctionExpression,
                                    start = 23,
                                    end = 55,
                                    id =  new IdentifierNode(new SourceLocation(new Position(1, 38, 38), new Position(1, 41, 41)), "foo"),
                                    generator = false,
                                    bexpression = false,
                                    async = true,
                                    @params = new List<Node>(),
                                    fbody = new Node
                                    {
                                        type = NodeType.BlockStatement,
                                        start = 44,
                                        end = 55,
                                        body = new List<Node>
                                        {
                                            new Node
                                            {
                                                type = NodeType.ExpressionStatement,
                                                start = 46,
                                                end = 53,
                                                expression = new Node
                                                {
                                                    type = NodeType.AwaitExpression,
                                                    start = 46,
                                                    end = 53,
                                                    argument = new IdentifierNode(new SourceLocation(new Position(1, 52, 52), new Position(1, 53, 53)), "b")
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        fbody = new Node
                        {
                            type = NodeType.BlockStatement,
                            start = 57,
                            end = 59,
                            body = new List<Node>()
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            Test("async function foo(a = async () => await b) {}", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 46,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.FunctionDeclaration,
                        start = 0,
                        end = 46,
                        id =  new IdentifierNode(new SourceLocation(new Position(1, 15, 15), new Position(1, 18, 18)), "foo"),
                        generator = false,
                        bexpression = false,
                        async = true,
                        @params = new List<Node>
                        {
                            new Node
                            {
                                type = NodeType.AssignmentPattern,
                                start = 19,
                                end = 42,
                                left = new IdentifierNode(new SourceLocation(new Position(1, 19, 19), new Position(1, 20, 20)), "a"),
                                right = new Node
                                {
                                    type = NodeType.ArrowFunctionExpression,
                                    start = 23,
                                    end = 42,
                                    id = null,
                                    generator = false,
                                    bexpression = true,
                                    async = true,
                                    @params = new List<Node>(),
                                    fbody = new Node
                                    {
                                        type = NodeType.AwaitExpression,
                                        start = 35,
                                        end = 42,
                                        argument = new IdentifierNode(new SourceLocation(new Position(1, 41, 41), new Position(1, 42, 42)), "b")
                                    }
                                }
                            }
                        },
                        fbody = new Node
                        {
                            type = NodeType.BlockStatement,
                            start = 44,
                            end = 46,
                            body = new List<Node>()
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            Test("async function foo(a = {async bar() { await b }}) {}", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 52,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.FunctionDeclaration,
                        start = 0,
                        end = 52,
                        id =  new IdentifierNode(new SourceLocation(new Position(1, 15, 15), new Position(1, 18, 18)), "foo"),
                        generator = false,
                        bexpression = false,
                        async = true,
                        @params = new List<Node>
                        {
                            new Node
                            {
                                type = NodeType.AssignmentPattern,
                                start = 19,
                                end = 48,
                                left = new IdentifierNode(new SourceLocation(new Position(1, 19, 19), new Position(1, 20, 20)), "a"),
                                right = new Node
                                {
                                    type = NodeType.ObjectExpression,
                                    start = 23,
                                    end = 48,
                                    properties = new List<Node>
                                    {
                                        new Node
                                        {
                                            type = NodeType.Property,
                                            start = 24,
                                            end = 47,
                                            method = true,
                                            shorthand = false,
                                            computed = false,
                                            key = new IdentifierNode(new SourceLocation(new Position(1, 30, 30), new Position(1, 33, 33)), "bar"),
                                            kind = "init",
                                            value = new Node
                                            {
                                                type = NodeType.FunctionExpression,
                                                start = 33,
                                                end = 47,
                                                id = null,
                                                generator = false,
                                                bexpression = false,
                                                async = true,
                                                @params = new List<Node>(),
                                                fbody = new Node
                                                {
                                                    type = NodeType.BlockStatement,
                                                    start = 36,
                                                    end = 47,
                                                    body = new List<Node>
                                                    {
                                                        new Node
                                                        {
                                                            type = NodeType.ExpressionStatement,
                                                            start = 38,
                                                            end = 45,
                                                            expression = new Node
                                                            {
                                                                type = NodeType.AwaitExpression,
                                                                start = 38,
                                                                end = 45,
                                                                argument = new IdentifierNode(new SourceLocation(new Position(1, 44, 44), new Position(1, 45, 45)), "b")
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        fbody = new Node
                        {
                            type = NodeType.BlockStatement,
                            start = 50,
                            end = 52,
                            body = new List<Node>()
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            Test("async function foo(a = class {async bar() { await b }}) {}", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 58,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.FunctionDeclaration,
                        start = 0,
                        end = 58,
                        id =  new IdentifierNode(new SourceLocation(new Position(1, 15, 15), new Position(1, 18, 18)), "foo"),
                        generator = false,
                        bexpression = false,
                        async = true,
                        @params = new List<Node>
                        {
                            new Node
                            {
                                type = NodeType.AssignmentPattern,
                                start = 19,
                                end = 54,
                                left = new IdentifierNode(new SourceLocation(new Position(1, 19, 19), new Position(1, 20, 20)), "a"),
                                right = new Node
                                {
                                    type = NodeType.ClassExpression,
                                    start = 23,
                                    end = 54,
                                    id = null,
                                    superClass = null,
                                    fbody = new Node
                                    {
                                        type = NodeType.ClassBody,
                                        start = 29,
                                        end = 54,
                                        body = new List<Node>
                                        {
                                            new Node
                                            {
                                                type = NodeType.MethodDefinition,
                                                start = 30,
                                                end = 53,
                                                computed = false,
                                                key = new IdentifierNode(new SourceLocation(new Position(1, 36, 36), new Position(1, 39, 39)), "bar"),
                                                @static = false,
                                                kind = "method",
                                                value = new Node
                                                {
                                                    type = NodeType.FunctionExpression,
                                                    start = 39,
                                                    end = 53,
                                                    id = null,
                                                    generator = false,
                                                    bexpression = false,
                                                    async = true,
                                                    @params = new List<Node>(),
                                                    fbody = new Node
                                                    {
                                                        type = NodeType.BlockStatement,
                                                        start = 42,
                                                        end = 53,
                                                        body = new List<Node>
                                                        {
                                                            new Node
                                                            {
                                                                type = NodeType.ExpressionStatement,
                                                                start = 44,
                                                                end = 51,
                                                                expression = new Node
                                                                {
                                                                    type = NodeType.AwaitExpression,
                                                                    start = 44,
                                                                    end = 51,
                                                                    argument = new IdentifierNode(new SourceLocation(new Position(1, 50, 50), new Position(1, 51, 51)), "b")
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        fbody = new Node
                        {
                            type = NodeType.BlockStatement,
                            start = 56,
                            end = 58,
                            body = new List<Node>()
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            // Distinguish ParenthesizedExpression or ArrowFunctionExpression
            Test("async function wrap() {\n(a = await b)\n}", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 39,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.FunctionDeclaration,
                        start = 0,
                        end = 39,
                        id =  new IdentifierNode(new SourceLocation(new Position(1, 15, 15), new Position(1, 19, 19)), "wrap"),
                        generator = false,
                        bexpression = false,
                        async = true,
                        @params = new List<Node>(),
                        fbody = new Node
                        {
                            type = NodeType.BlockStatement,
                            start = 22,
                            end = 39,
                            body = new List<Node>
                            {
                                new Node
                                {
                                    type = NodeType.ExpressionStatement,
                                    start = 24,
                                    end = 37,
                                    expression = new Node
                                    {
                                        type = NodeType.AssignmentExpression,
                                        start = 25,
                                        end = 36,
                                        @operator = "=",
                                        left = new IdentifierNode(new SourceLocation(new Position(2, 1, 25), new Position(2, 2, 26)), "a"),
                                        right = new Node
                                        {
                                            type = NodeType.AwaitExpression,
                                            start = 29,
                                            end = 36,
                                            argument = new IdentifierNode(new SourceLocation(new Position(2, 11, 35), new Position(2, 12, 36)), "b")
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});
            testFail("async function wrap() {\n(a = await b) => a\n}", "Await expression cannot be a default value (2:5)", new Options {ecmaVersion = 8});

            Test("async function wrap() {\n({a = await b} = obj)\n}", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 47,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.FunctionDeclaration,
                        start = 0,
                        end = 47,
                        id =  new IdentifierNode(new SourceLocation(new Position(1, 15, 15), new Position(1, 19, 19)), "wrap"),
                        generator = false,
                        bexpression = false,
                        async = true,
                        @params = new List<Node>(),
                        fbody = new Node
                        {
                            type = NodeType.BlockStatement,
                            start = 22,
                            end = 47,
                            body = new List<Node>
                            {
                                new Node
                                {
                                    type = NodeType.ExpressionStatement,
                                    start = 24,
                                    end = 45,
                                    expression = new Node
                                    {
                                        type = NodeType.AssignmentExpression,
                                        start = 25,
                                        end = 44,
                                        @operator = "=",
                                        left = new Node
                                        {
                                            type = NodeType.ObjectPattern,
                                            start = 25,
                                            end = 38,
                                            properties = new List<Node>
                                            {
                                                new Node
                                                {
                                                    type = NodeType.Property,
                                                    start = 26,
                                                    end = 37,
                                                    method = false,
                                                    shorthand = true,
                                                    computed = false,
                                                    key = new IdentifierNode(new SourceLocation(new Position(2, 2, 26), new Position(2, 3, 27)), "a"),
                                                    kind = "init",
                                                    value = new Node
                                                    {
                                                        type = NodeType.AssignmentPattern,
                                                        start = 26,
                                                        end = 37,
                                                        left = new IdentifierNode(new SourceLocation(new Position(2, 2, 26), new Position(2, 3, 27)), "a"),
                                                        right = new Node
                                                        {
                                                            type = NodeType.AwaitExpression,
                                                            start = 30,
                                                            end = 37,
                                                            argument = new IdentifierNode(new SourceLocation(new Position(2, 12, 36), new Position(2, 13, 37)), "b")
                                                        }
                                                    }
                                                }
                                            }
                                        },
                                        right = new IdentifierNode(new SourceLocation(new Position(2, 17, 41), new Position(2, 20, 44)), "obj")
                                    }
                                }
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});
            testFail("async function wrap() {\n({a = await b} = obj) => a\n}", "Await expression cannot be a default value (2:6)", new Options {ecmaVersion = 8});

            Test("function* wrap() {\nasync(a = yield b)\n}", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 39,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.FunctionDeclaration,
                        start = 0,
                        end = 39,
                        id =  new IdentifierNode(new SourceLocation(new Position(1, 10, 10), new Position(1, 14, 14)), "wrap"),
                        @params = new List<Node>(),
                        generator = true,
                        bexpression = false,
                        async = false,
                        fbody = new Node
                        {
                            type = NodeType.BlockStatement,
                            start = 17,
                            end = 39,
                            body = new List<Node>
                            {
                                new Node
                                {
                                    type = NodeType.ExpressionStatement,
                                    start = 19,
                                    end = 37,
                                    expression = new Node
                                    {
                                        type = NodeType.CallExpression,
                                        start = 19,
                                        end = 37,
                                        callee = new IdentifierNode(new SourceLocation(new Position(2, 0, 19), new Position(2, 5, 24)), "async"),
                                        arguments = new List<Node>
                                        {
                                            new Node
                                            {
                                                type = NodeType.AssignmentExpression,
                                                start = 25,
                                                end = 36,
                                                @operator = "=",
                                                left = new IdentifierNode(new SourceLocation(new Position(2, 6, 25), new Position(2, 7, 26)), "a"),
                                                right = new Node
                                                {
                                                    type = NodeType.YieldExpression,
                                                    start = 29,
                                                    end = 36,
                                                    @delegate = false,
                                                    argument = new IdentifierNode(new SourceLocation(new Position(2, 16, 35), new Position(2, 17, 36)), "b")
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});
            testFail("function* wrap() {\nasync(a = yield b) => a\n}", "Yield expression cannot be a default value (2:10)", new Options {ecmaVersion = 8});

            // https://github.com/ternjs/acorn/issues/464
            Test("f = ({ w = counter(), x = counter(), y = counter(), z = counter() } = { w: null, x: 0, y: false, z: '' }) => {}", new Node
            {
                type = NodeType.Program,
                start = 0,
                end = 111,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExpressionStatement,
                        start = 0,
                        end = 111,
                        expression = new Node
                        {
                            type = NodeType.AssignmentExpression,
                            start = 0,
                            end = 111,
                            @operator = "=",
                            left = new IdentifierNode(new SourceLocation(new Position(1, 0, 0), new Position(1, 1, 1)), "f"),
                            right = new Node
                            {
                                type = NodeType.ArrowFunctionExpression,
                                start = 4,
                                end = 111,
                                id = null,
                                @params = new List<Node>
                                {
                                    new Node
                                    {
                                        type = NodeType.AssignmentPattern,
                                        start = 5,
                                        end = 104,
                                        left = new Node
                                        {
                                            type = NodeType.ObjectPattern,
                                            start = 5,
                                            end = 67,
                                            properties = new List<Node>
                                            {
                                                new Node
                                                {
                                                    type = NodeType.Property,
                                                    start = 7,
                                                    end = 20,
                                                    method = false,
                                                    shorthand = true,
                                                    computed = false,
                                                    key = new IdentifierNode(new SourceLocation(new Position(1, 7, 7), new Position(1, 8, 8)), "w"),
                                                    kind = "init",
                                                    value = new Node
                                                    {
                                                        type = NodeType.AssignmentPattern,
                                                        start = 7,
                                                        end = 20,
                                                        left = new IdentifierNode(new SourceLocation(new Position(1, 7, 7), new Position(1, 8, 8)), "w"),
                                                        right = new Node
                                                        {
                                                            type = NodeType.CallExpression,
                                                            start = 11,
                                                            end = 20,
                                                            callee = new IdentifierNode(new SourceLocation(new Position(1, 11, 11), new Position(1, 18, 18)), "counter"),
                                                            arguments = new List<Node>()
                                                        }
                                                    }
                                                },
                                                new Node
                                                {
                                                    type = NodeType.Property,
                                                    start = 22,
                                                    end = 35,
                                                    method = false,
                                                    shorthand = true,
                                                    computed = false,
                                                    key = new IdentifierNode(new SourceLocation(new Position(1, 22, 22), new Position(1, 23, 23)), "x"),
                                                    kind = "init",
                                                    value = new Node
                                                    {
                                                        type = NodeType.AssignmentPattern,
                                                        start = 22,
                                                        end = 35,
                                                        left = new IdentifierNode(new SourceLocation(new Position(1, 22, 22), new Position(1, 23, 23)), "x"),
                                                        right = new Node
                                                        {
                                                            type = NodeType.CallExpression,
                                                            start = 26,
                                                            end = 35,
                                                            callee = new IdentifierNode(new SourceLocation(new Position(1, 26, 26), new Position(1, 33, 33)), "counter"),
                                                            arguments = new List<Node>()
                                                        }
                                                    }
                                                },
                                                new Node
                                                {
                                                    type = NodeType.Property,
                                                    start = 37,
                                                    end = 50,
                                                    method = false,
                                                    shorthand = true,
                                                    computed = false,
                                                    key = new IdentifierNode(new SourceLocation(new Position(1, 37, 37), new Position(1, 38, 38)), "y"),
                                                    kind = "init",
                                                    value = new Node
                                                    {
                                                        type = NodeType.AssignmentPattern,
                                                        start = 37,
                                                        end = 50,
                                                        left = new IdentifierNode(new SourceLocation(new Position(1, 37, 37), new Position(1, 38, 38)), "y"),
                                                        right = new Node
                                                        {
                                                            type = NodeType.CallExpression,
                                                            start = 41,
                                                            end = 50,
                                                            callee = new IdentifierNode(new SourceLocation(new Position(1, 41, 41), new Position(1, 48, 48)), "counter"),
                                                            arguments = new List<Node>()
                                                        }
                                                    }
                                                },
                                                new Node
                                                {
                                                    type = NodeType.Property,
                                                    start = 52,
                                                    end = 65,
                                                    method = false,
                                                    shorthand = true,
                                                    computed = false,
                                                    key = new IdentifierNode(new SourceLocation(new Position(1, 52, 52), new Position(1, 53, 53)), "z"),
                                                    kind = "init",
                                                    value = new Node
                                                    {
                                                        type = NodeType.AssignmentPattern,
                                                        start = 52,
                                                        end = 65,
                                                        left = new IdentifierNode(new SourceLocation(new Position(1, 52, 52), new Position(1, 53, 53)), "z"),
                                                        right = new Node
                                                        {
                                                            type = NodeType.CallExpression,
                                                            start = 56,
                                                            end = 65,
                                                            callee = new IdentifierNode(new SourceLocation(new Position(1, 56, 56), new Position(1, 63, 63)), "counter"),
                                                            arguments = new List<Node>()
                                                        }
                                                    }
                                                }
                                            }
                                        },
                                        right = new Node
                                        {
                                            type = NodeType.ObjectExpression,
                                            start = 70,
                                            end = 104,
                                            properties = new List<Node>
                                            {
                                                new Node
                                                {
                                                    type = NodeType.Property,
                                                    start = 72,
                                                    end = 79,
                                                    method = false,
                                                    shorthand = false,
                                                    computed = false,
                                                    key = new IdentifierNode(new SourceLocation(new Position(1, 72, 72), new Position(1, 73, 73)), "w"),
                                                    kind = "init",
                                                    value = new Node
                                                    {
                                                        type = NodeType.Literal,
                                                        start = 75,
                                                        end = 79,
                                                        value = null,
                                                        raw = "null"
                                                    }
                                                },
                                                new Node
                                                {
                                                    type = NodeType.Property,
                                                    start = 81,
                                                    end = 85,
                                                    method = false,
                                                    shorthand = false,
                                                    computed = false,
                                                    key = new IdentifierNode(new SourceLocation(new Position(1, 81, 81), new Position(1, 82, 82)), "x"),
                                                    kind = "init",
                                                    value = new Node
                                                    {
                                                        type = NodeType.Literal,
                                                        start = 84,
                                                        end = 85,
                                                        value = 0,
                                                        raw = "0"
                                                    }
                                                },
                                                new Node
                                                {
                                                    type = NodeType.Property,
                                                    start = 87,
                                                    end = 95,
                                                    method = false,
                                                    shorthand = false,
                                                    computed = false,
                                                    key = new IdentifierNode(new SourceLocation(new Position(1, 87, 87), new Position(1, 88, 88)), "y"),
                                                    kind = "init",
                                                    value = new Node
                                                    {
                                                        type = NodeType.Literal,
                                                        start = 90,
                                                        end = 95,
                                                        value = false,
                                                        raw = "false"
                                                    }
                                                },
                                                new Node
                                                {
                                                    type = NodeType.Property,
                                                    start = 97,
                                                    end = 102,
                                                    method = false,
                                                    shorthand = false,
                                                    computed = false,
                                                    key = new IdentifierNode(new SourceLocation(new Position(1, 97, 97), new Position(1, 98, 98)), "z"),
                                                    kind = "init",
                                                    value = new Node
                                                    {
                                                        type = NodeType.Literal,
                                                        start = 100,
                                                        end = 102,
                                                        value = "",
                                                        raw = "''"
                                                    }
                                                }
                                            }
                                        }
                                    }
                                },
                                generator = false,
                                bexpression = false,
                                async = false,
                                fbody = new Node
                                {
                                    type = NodeType.BlockStatement,
                                    start = 109,
                                    end = 111,
                                    body = new List<Node>()
                                }
                            }
                        }
                    }
                },
                sourceType = "script"
            }, new Options {ecmaVersion = 8});

            Test("({ async: true })", new Node
            {
                type = NodeType.Program,
                body = new List<Node>
                {
                    new Node
                    {
                        type = NodeType.ExpressionStatement,
                        expression = new Node
                        {
                            type = NodeType.ObjectExpression,
                            properties = new List<Node>
                            {
                                new Node
                                {
                                    type = NodeType.Property,
                                    key = new IdentifierNode(default, "async"),
                                    value = new Node
                                    {
                                        type = NodeType.Literal,
                                        value = true
                                    },
                                    kind = "init"
                                }
                            }
                        }
                    }
                }
            }, new Options {ecmaVersion = 8});

            // Tests for B.3.4 FunctionDeclarations in IfStatement Statement Clauses
            Test("if (x) async function f() {}", new Node
                {
                    type = NodeType.Program,
                    body = new List<Node>
                    {
                        new Node
                        {
                            type = NodeType.IfStatement,
                            consequent = new Node
                            {
                                type = NodeType.FunctionDeclaration
                            },
                            alternate = null
                        }
                    }
                },
                new Options {ecmaVersion = 8}
            );

            testFail("(async)(a) => 12", "Unexpected token (1:11)", new Options {ecmaVersion = 8});

            testFail("f = async ((x)) => x", "Parenthesized pattern (1:11)", new Options {ecmaVersion = 8});

            // allow 'async' as a shorthand property in script.
            Test("({async})", new Node
                {
                    type = NodeType.Program,
                    start = 0,
                    end = 9,
                    body = new List<Node>
                    {
                        new Node
                        {
                            type = NodeType.ExpressionStatement,
                            start = 0,
                            end = 9,
                            expression = new Node
                            {
                                type = NodeType.ObjectExpression,
                                start = 1,
                                end = 8,
                                properties = new List<Node>
                                {
                                    new Node
                                    {
                                        type = NodeType.Property,
                                        start = 2,
                                        end = 7,
                                        method = false,
                                        shorthand = true,
                                        computed = false,
                                        key = new IdentifierNode(new SourceLocation(new Position(1, 2, 2), new Position(1, 7, 7)), "async"),
                                        kind = "init",
                                        value = new IdentifierNode(new SourceLocation(new Position(1, 2, 2), new Position(1, 7, 7)), "async")
                                    }
                                }
                            }
                        }
                    },
                    sourceType = "script"
                },
                new Options {ecmaVersion = 8}
            );

            Test("({async, foo})", new Node
                {
                    type = NodeType.Program,
                    start = 0,
                    end = 14,
                    body = new List<Node>
                    {
                        new Node
                        {
                            type = NodeType.ExpressionStatement,
                            start = 0,
                            end = 14,
                            expression = new Node
                            {
                                type = NodeType.ObjectExpression,
                                start = 1,
                                end = 13,
                                properties = new List<Node>
                                {
                                    new Node
                                    {
                                        type = NodeType.Property,
                                        start = 2,
                                        end = 7,
                                        method = false,
                                        shorthand = true,
                                        computed = false,
                                        key = new IdentifierNode(new SourceLocation(new Position(1, 2, 2), new Position(1, 7, 7)), "async"),
                                        kind = "init",
                                        value = new IdentifierNode(new SourceLocation(new Position(1, 2, 2), new Position(1, 7, 7)), "async")
                                    },
                                    new Node
                                    {
                                        type = NodeType.Property,
                                        start = 9,
                                        end = 12,
                                        method = false,
                                        shorthand = true,
                                        computed = false,
                                        key = new IdentifierNode(new SourceLocation(new Position(1, 9, 9), new Position(1, 12, 12)), "foo"),
                                        kind = "init",
                                        value = new IdentifierNode(new SourceLocation(new Position(1, 9, 9), new Position(1, 12, 12)), "foo")
                                    }
                                }
                            }
                        }
                    },
                    sourceType = "script"
                },
                new Options {ecmaVersion = 8}
            );

            Test("({async = 0} = {})", new Node
                {
                    type = NodeType.Program,
                    start = 0,
                    end = 18,
                    body = new List<Node>
                    {
                        new Node
                        {
                            type = NodeType.ExpressionStatement,
                            start = 0,
                            end = 18,
                            expression = new Node
                            {
                                type = NodeType.AssignmentExpression,
                                start = 1,
                                end = 17,
                                @operator = "=",
                                left = new Node
                                {
                                    type = NodeType.ObjectPattern,
                                    start = 1,
                                    end = 12,
                                    properties = new List<Node>
                                    {
                                        new Node
                                        {
                                            type = NodeType.Property,
                                            start = 2,
                                            end = 11,
                                            method = false,
                                            shorthand = true,
                                            computed = false,
                                            key = new IdentifierNode(new SourceLocation(new Position(1, 2, 2), new Position(1, 7, 7)), "async"),
                                            kind = "init",
                                            value = new Node
                                            {
                                                type = NodeType.AssignmentPattern,
                                                start = 2,
                                                end = 11,
                                                left = new IdentifierNode(new SourceLocation(new Position(1, 2, 2), new Position(1, 7, 7)), "async"),
                                                right = new Node
                                                {
                                                    type = NodeType.Literal,
                                                    start = 10,
                                                    end = 11,
                                                    value = 0,
                                                    raw = "0"
                                                }
                                            }
                                        }
                                    }
                                },
                                right = new Node
                                {
                                    type = NodeType.ObjectExpression,
                                    start = 15,
                                    end = 17,
                                    properties = new List<Node>()
                                }
                            }
                        }
                    },
                    sourceType = "script"
                },
                new Options {ecmaVersion = 8}
            );

            // async functions with vary names.
            Test("({async \"foo\"(){}})", new Node
                {
                    type = NodeType.Program,
                    start = 0,
                    end = 19,
                    body = new List<Node>
                    {
                        new Node
                        {
                            type = NodeType.ExpressionStatement,
                            start = 0,
                            end = 19,
                            expression = new Node
                            {
                                type = NodeType.ObjectExpression,
                                start = 1,
                                end = 18,
                                properties = new List<Node>
                                {
                                    new Node
                                    {
                                        type = NodeType.Property,
                                        start = 2,
                                        end = 17,
                                        method = true,
                                        shorthand = false,
                                        computed = false,
                                        key = new Node
                                        {
                                            type = NodeType.Literal,
                                            start = 8,
                                            end = 13,
                                            value = "foo",
                                            raw = "\"foo\""
                                        },
                                        kind = "init",
                                        value = new Node
                                        {
                                            type = NodeType.FunctionExpression,
                                            start = 13,
                                            end = 17,
                                            id = null,
                                            generator = false,
                                            bexpression = false,
                                            async = true,
                                            @params = new List<Node>(),
                                            fbody = new Node
                                            {
                                                type = NodeType.BlockStatement,
                                                start = 15,
                                                end = 17,
                                                body = new List<Node>()
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    sourceType = "script"
                },
                new Options {ecmaVersion = 8}
            );

            Test("({async 'foo'(){}})", new Node
                {
                    type = NodeType.Program,
                    start = 0,
                    end = 19,
                    body = new List<Node>
                    {
                        new Node
                        {
                            type = NodeType.ExpressionStatement,
                            start = 0,
                            end = 19,
                            expression = new Node
                            {
                                type = NodeType.ObjectExpression,
                                start = 1,
                                end = 18,
                                properties = new List<Node>
                                {
                                    new Node
                                    {
                                        type = NodeType.Property,
                                        start = 2,
                                        end = 17,
                                        method = true,
                                        shorthand = false,
                                        computed = false,
                                        key = new Node
                                        {
                                            type = NodeType.Literal,
                                            start = 8,
                                            end = 13,
                                            value = "foo",
                                            raw = "'foo'"
                                        },
                                        kind = "init",
                                        value = new Node
                                        {
                                            type = NodeType.FunctionExpression,
                                            start = 13,
                                            end = 17,
                                            id = null,
                                            generator = false,
                                            bexpression = false,
                                            async = true,
                                            @params = new List<Node>(),
                                            fbody = new Node
                                            {
                                                type = NodeType.BlockStatement,
                                                start = 15,
                                                end = 17,
                                                body = new List<Node>()
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    sourceType = "script"
                },
                new Options {ecmaVersion = 8}
            );

            Test("({async 100(){}})", new Node
                {
                    type = NodeType.Program,
                    start = 0,
                    end = 17,
                    body = new List<Node>
                    {
                        new Node
                        {
                            type = NodeType.ExpressionStatement,
                            start = 0,
                            end = 17,
                            expression = new Node
                            {
                                type = NodeType.ObjectExpression,
                                start = 1,
                                end = 16,
                                properties = new List<Node>
                                {
                                    new Node
                                    {
                                        type = NodeType.Property,
                                        start = 2,
                                        end = 15,
                                        method = true,
                                        shorthand = false,
                                        computed = false,
                                        key = new Node
                                        {
                                            type = NodeType.Literal,
                                            start = 8,
                                            end = 11,
                                            value = 100,
                                            raw = "100"
                                        },
                                        kind = "init",
                                        value = new Node
                                        {
                                            type = NodeType.FunctionExpression,
                                            start = 11,
                                            end = 15,
                                            id = null,
                                            generator = false,
                                            bexpression = false,
                                            async = true,
                                            @params = new List<Node>(),
                                            fbody = new Node
                                            {
                                                type = NodeType.BlockStatement,
                                                start = 13,
                                                end = 15,
                                                body = new List<Node>()
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    sourceType = "script"
                },
                new Options {ecmaVersion = 8}
            );

            Test("({async [foo](){}})", new Node
                {
                    type = NodeType.Program,
                    start = 0,
                    end = 19,
                    body = new List<Node>
                    {
                        new Node
                        {
                            type = NodeType.ExpressionStatement,
                            start = 0,
                            end = 19,
                            expression = new Node
                            {
                                type = NodeType.ObjectExpression,
                                start = 1,
                                end = 18,
                                properties = new List<Node>
                                {
                                    new Node
                                    {
                                        type = NodeType.Property,
                                        start = 2,
                                        end = 17,
                                        method = true,
                                        shorthand = false,
                                        computed = true,
                                        key = new IdentifierNode(new SourceLocation(new Position(1, 9, 9), new Position(1, 12, 12)), "foo"),
                                        kind = "init",
                                        value = new Node
                                        {
                                            type = NodeType.FunctionExpression,
                                            start = 13,
                                            end = 17,
                                            id = null,
                                            generator = false,
                                            bexpression = false,
                                            async = true,
                                            @params = new List<Node>(),
                                            fbody = new Node
                                            {
                                                type = NodeType.BlockStatement,
                                                start = 15,
                                                end = 17,
                                                body = new List<Node>()
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    sourceType = "script"
                },
                new Options {ecmaVersion = 8}
            );

            Test("({ async delete() {} })", new Node { }, new Options {ecmaVersion = 8});
        }
    }
}
