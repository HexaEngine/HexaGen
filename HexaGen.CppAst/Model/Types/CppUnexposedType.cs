// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using HexaGen.CppAst.Collections;
using HexaGen.CppAst.Model.Interfaces;
using System;
using System.Collections.Generic;

namespace HexaGen.CppAst.Model.Types
{
    /// <summary>
    /// A type not fully/correctly exposed by the C++ parser.
    /// </summary>
    /// <remarks>
    /// Template parameter type instance are actually exposed with this type.
    /// </remarks>
    public sealed class CppUnexposedType : CppType, ICppTemplateOwner, ICppContainer
    {
        /// <summary>
        /// Creates an instance of this type.
        /// </summary>
        /// <param name="name">Fullname of the unexposed type</param>
        public CppUnexposedType(string name) : base(CppTypeKind.Unexposed)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            TemplateParameters = new CppContainerList<CppType>(this);
        }

        /// <summary>
        /// Full name of the unexposed type
        /// </summary>
        public string Name { get; }

        /// <inheritdoc />
        public override int SizeOf { get; set; }

        /// <inheritdoc />
        public CppContainerList<CppType> TemplateParameters { get; }

        /// <inheritdoc />
        public override CppType GetCanonicalType() => this;

        /// <inheritdoc />
        public override string ToString() => Name;

        public IEnumerable<ICppDeclaration> Children()
        {
            yield break;
        }
    }
}