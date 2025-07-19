// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using HexaGen.CppAst.Model.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace HexaGen.CppAst.Model.Declarations
{
    /// <summary>
    /// A C++ function type (e.g `void (*)(int arg1, int arg2)`)
    /// </summary>
    public sealed class CppFunctionType : CppFunctionTypeBase
    {
        /// <summary>
        /// Constructor of a function type.
        /// </summary>
        /// <param name="returnType">Return type of this function type.</param>
        public CppFunctionType(CppType returnType) : base(CppTypeKind.Function, returnType)
        {
        }
    }
}