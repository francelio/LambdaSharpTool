/*
 * LambdaSharp (λ#)
 * Copyright (C) 2018-2020
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

namespace LambdaSharp.Compiler.Syntax.Expressions {

    public sealed class JoinFunctionExpression : AFunctionExpression {

        // !Join [ STRING, VALUE ]
        // NOTE: For the Fn::Join delimiter, you cannot use any functions. You must specify a string value.
        //  For the Fn::Join list of values, you can use the following functions:
        //  - Fn::Base64
        //  - Fn::FindInMap
        //  - Fn::GetAtt
        //  - Fn::GetAZs
        //  - Fn::If
        //  - Fn::ImportValue
        //  - Fn::Join
        //  - Fn::Split
        //  - Fn::Select
        //  - Fn::Sub
        //  - Ref

        //--- Fields ---
        private LiteralExpression? _delimiter;
        private AExpression? _values;

        //--- Properties ---

        // TODO: allow AExpression, but then validate that after optimization it is a string literal
        [SyntaxHidden]
        public LiteralExpression Delimiter {
            get => _delimiter ?? throw new InvalidOperationException();
            set => _delimiter = Adopt(value ?? throw new ArgumentNullException());
        }

        [SyntaxHidden]
        public AExpression Values {
            get => _values ?? throw new InvalidOperationException();
            set => _values = Adopt(value ?? throw new ArgumentNullException());
        }

        //--- Methods ---
        public override ASyntaxNode CloneNode() => new JoinFunctionExpression {
            Delimiter = Delimiter.Clone(),
            Values = Values.Clone()
        };
    }
}