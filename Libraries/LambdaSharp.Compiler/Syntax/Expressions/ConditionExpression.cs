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
using LambdaSharp.Compiler.Exceptions;
using LambdaSharp.Compiler.Syntax.Declarations;

namespace LambdaSharp.Compiler.Syntax.Expressions {

    public sealed class ConditionExpression : AConditionExpression {

        // !Condition STRING

        //--- Fields ---
        private LiteralExpression? _referenceName;
        private ConditionDeclaration? _referencedDeclaration;

        //--- Properties ---
        public LiteralExpression ReferenceName {
            get => _referenceName ?? throw new InvalidOperationException();
            set => _referenceName = SetParent(value ?? throw new ArgumentNullException());
        }

        public ConditionDeclaration? ReferencedDeclaration {
            get => _referencedDeclaration;
            set {
                if(_referencedDeclaration != null) {
                    _referencedDeclaration.UntrackDependency(this);
                }
                _referencedDeclaration = value;
                if(_referencedDeclaration != null) {
                    ParentItemDeclaration?.TrackDependency(_referencedDeclaration, this);
                }
            }
        }

        //--- Methods ---
        public override ASyntaxNode? VisitNode(ISyntaxVisitor visitor) {
            if(!visitor.VisitStart(this)) {
                return this;
            }
            ReferenceName = ReferenceName.Visit(visitor) ?? throw new NullValueException();
            return visitor.VisitEnd(this);
        }

        public override void InspectNode(Action<ASyntaxNode> inspector) {
            inspector(this);
            ReferenceName.InspectNode(inspector);
        }

        public override ASyntaxNode CloneNode() => new ConditionExpression {
            ReferenceName = ReferenceName.Clone(),
            ReferencedDeclaration = ReferencedDeclaration
        };
    }
}