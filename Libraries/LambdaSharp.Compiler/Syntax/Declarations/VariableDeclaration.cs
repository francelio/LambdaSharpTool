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

using System;
using LambdaSharp.Compiler.Syntax.Expressions;

namespace LambdaSharp.Compiler.Syntax.Declarations {

    [SyntaxDeclarationKeyword("Variable")]
    public sealed class VariableDeclaration : AItemDeclaration, IScopedDeclaration {

        //--- Fields ---
        private LiteralExpression? type;
        private SyntaxNodeCollection<LiteralExpression> _scope;
        private AExpression? _value;
        private ObjectExpression? _encryptionContext;

        //--- Constructors ---
        public VariableDeclaration(LiteralExpression itemName) : base(itemName) {
            _scope = SetParent(new SyntaxNodeCollection<LiteralExpression>());
        }

        //--- Properties ---

        [SyntaxOptional]
        public LiteralExpression? Type {
            get => type;
            set => type = SetParent(value);
        }

        [SyntaxOptional]
        public SyntaxNodeCollection<LiteralExpression> Scope {
            get => _scope;
            set => _scope = SetParent(value ?? throw new ArgumentNullException());
        }

        [SyntaxRequired]
        public AExpression? Value {
            get => _value;
            set => _value = SetParent(value);
        }

        [SyntaxOptional]
        public ObjectExpression? EncryptionContext {
            get => _encryptionContext;
            set => _encryptionContext = SetParent(value);
        }

        public bool HasSecretType => Type!.Value == "Secret";

        //--- Methods ---
        public override ASyntaxNode? VisitNode(ISyntaxVisitor visitor) {
            if(!visitor.VisitStart(this)) {
                return this;
            }
            AssertIsSame(ItemName, ItemName.Visit(visitor));
            Type = Type?.Visit(visitor);
            Scope = Scope.Visit(visitor);
            Value = Value?.Visit(visitor);
            EncryptionContext = EncryptionContext?.Visit(visitor);
            Declarations = Declarations.Visit(visitor);
            return visitor.VisitEnd(this);
        }
        public override void InspectNode(Action<ASyntaxNode> inspector) {
            inspector(this);
            ItemName.InspectNode(inspector);
            Type?.InspectNode(inspector);
            Scope.InspectNode(inspector);
            Value?.InspectNode(inspector);
            EncryptionContext?.InspectNode(inspector);
            Declarations.InspectNode(inspector);
        }
    }
}