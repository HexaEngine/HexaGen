// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using HexaGen.CppAst.Collections;
using HexaGen.CppAst.Model.Types;
using System.Collections.Generic;

namespace HexaGen.CppAst.Model.Interfaces
{
    /// <summary>
    /// Base interface of a type/method declared with template parameters.
    /// </summary>
    public interface ICppTemplateOwner
    {
        /// <summary>
        /// List of template parameters.
        /// </summary>
        CppContainerList<CppType> TemplateParameters { get; }
    }
}