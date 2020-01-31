/*
 * LambdaSharp (λ#)
 * Copyright (C) 2018-2019
 * lambdasharp.net
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace LambdaSharp.Tool.Compiler.Parser.Syntax {

    public abstract class ADeclaration : ASyntaxNode { }

    [SyntaxDeclarationKeyword("Module")]
    public class ModuleDeclaration : ADeclaration {

        //--- Fields ---
        private LiteralExpression _version;
        private LiteralExpression? _description;
        private ListExpression _pragmas;
        private SyntaxNodeCollection<LiteralExpression> _secrets;
        private SyntaxNodeCollection<UsingModuleDeclaration> _using;
        private SyntaxNodeCollection<AItemDeclaration> _items;

        //--- Constructors ---
        public ModuleDeclaration(LiteralExpression moduleName) {
            ModuleName = SetParent(moduleName) ?? throw new ArgumentNullException(nameof(moduleName));
            _version = SetParent(ASyntaxAnalyzer.Literal("1.0-DEV"));
            _pragmas = SetParent(new ListExpression());
            _secrets = SetParent(new SyntaxNodeCollection<LiteralExpression>());
            _using = SetParent(new SyntaxNodeCollection<UsingModuleDeclaration>());
            _items = SetParent(new SyntaxNodeCollection<AItemDeclaration>());
        }

        //--- Properties ---

        [SyntaxOptional]
        public LiteralExpression Version {
            get => _version;
            set => _version = SetParent(value) ?? throw new ArgumentNullException();
        }

        [SyntaxOptional]
        public LiteralExpression? Description {
            get => _description;
            set => _description = SetParent(value);
        }

        [SyntaxOptional]
        public ListExpression Pragmas {
            get => _pragmas;
            set => _pragmas = SetParent(value) ?? throw new ArgumentNullException();
        }

        [SyntaxOptional]
        public SyntaxNodeCollection<LiteralExpression> Secrets {
            get => _secrets;
            set => _secrets = SetParent(value) ?? throw new ArgumentNullException();
        }

        [SyntaxOptional]
        public SyntaxNodeCollection<UsingModuleDeclaration> Using {
            get => _using;
            set => _using = SetParent(value) ?? throw new ArgumentNullException();
        }

        [SyntaxRequired]
        public SyntaxNodeCollection<AItemDeclaration> Items {
            get => _items;
            set => _items = SetParent(value) ?? throw new ArgumentNullException();
        }

        public LiteralExpression ModuleName { get; }
        public bool HasPragma(string pragma) => Pragmas.Any(expression => (expression is LiteralExpression literalExpression) && (literalExpression.Value == pragma));
        public bool HasLambdaSharpDependencies => !HasPragma("no-lambdasharp-dependencies");
        public bool HasModuleRegistration => !HasPragma("no-module-registration");

        //--- Methods ---
        public override void Visit(ASyntaxNode? parent, ISyntaxVisitor visitor) {
            visitor.VisitStart(parent, this);
            ModuleName.Visit(this, visitor);
            Version?.Visit(this, visitor);
            Description?.Visit(this, visitor);
            Secrets?.Visit(this, visitor);
            Using?.Visit(this, visitor);
            Items?.Visit(this, visitor);
            visitor.VisitEnd(parent, this);
        }
    }

    [SyntaxDeclarationKeyword("Module")]
    public class UsingModuleDeclaration : ADeclaration {

        //--- Fields ---
        private LiteralExpression? _description;

        //--- Constructors ---
        public UsingModuleDeclaration(LiteralExpression moduleName) => ModuleName = SetParent(moduleName) ?? throw new ArgumentNullException(nameof(moduleName));

        //--- Properties ---

        [SyntaxOptional]
        public LiteralExpression? Description {
            get => _description;
            set => _description = SetParent(value);
        }

        public LiteralExpression ModuleName { get; }

        //--- Methods ---
        public override void Visit(ASyntaxNode parent, ISyntaxVisitor visitor) {
            visitor.VisitStart(parent, this);
            ModuleName.Visit(this, visitor);
            Description?.Visit(this, visitor);
            visitor.VisitEnd(parent, this);
        }
    }
}