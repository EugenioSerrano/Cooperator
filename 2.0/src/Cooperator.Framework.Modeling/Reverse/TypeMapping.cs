/*-
*       Copyright (c) 2006-2007 Eugenio Serrano, Daniel Calvin.
*       All rights reserved.
*
*       Redistribution and use in source and binary forms, with or without
*       modification, are permitted provided that the following conditions
*       are met:
*       1. Redistributions of source code must retain the above copyright
*          notice, this list of conditions and the following disclaimer.
*       2. Redistributions in binary form must reproduce the above copyright
*          notice, this list of conditions and the following disclaimer in the
*          documentation and/or other materials provided with the distribution.
*       3. Neither the name of copyright holders nor the names of its
*          contributors may be used to endorse or promote products derived
*          from this software without specific prior written permission.
*
*       THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
*       "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
*       TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
*       PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL COPYRIGHT HOLDERS OR CONTRIBUTORS
*       BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
*       CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
*       SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
*       INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
*       CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
*       ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
*       POSSIBILITY OF SUCH DAMAGE.
*/

// Código NUEVO de la 2.0 (no portado del 1.4): conserva la nulabilidad ACTIVA del árbol (SPEC-261016-0004 §A.3).

using System;
using System.Collections.Generic;

namespace Cooperator.Framework.Modeling.Reverse
{
    /// <summary>
    /// Regla de longitud por defecto de un tipo nativo.
    /// Es <b>dato</b> de la tabla, no un <c>switch</c> duplicado: la resuelve <see cref="MSSQLTypeConverter"/>.
    /// </summary>
    public enum NativeLengthRule
    {
        /// <summary>El tipo no tiene longitud (la regla del 1.4 devolvía <c>null</c>).</summary>
        None = 0,

        /// <summary>La longitud se conserva tal cual llega (incluido <c>-1</c> = máxima).</summary>
        AsIs = 1,

        /// <summary>La longitud se divide por 2 (tipos Unicode), conservando <c>-1</c>.</summary>
        Half = 2
    }

    /// <summary>
    /// Una entrada de la tabla del mapeo tipo nativo → tipo CLR.
    /// </summary>
    public sealed class NativeTypeMapping
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nativeType">Nombre del tipo nativo, en minúsculas.</param>
        /// <param name="clrType">Tipo CLR al que se resuelve.</param>
        /// <param name="lengthRule">Regla de longitud por defecto.</param>
        public NativeTypeMapping(string nativeType, Type clrType, NativeLengthRule lengthRule)
        {
            NativeType = nativeType;
            ClrType = clrType;
            LengthRule = lengthRule;
        }

        /// <summary>Nombre del tipo nativo.</summary>
        public string NativeType { get; }

        /// <summary>Tipo CLR correspondiente.</summary>
        public Type ClrType { get; }

        /// <summary>Regla de longitud por defecto.</summary>
        public NativeLengthRule LengthRule { get; }
    }

    /// <summary>
    /// <b>La</b> fuente única del mapeo SQL↔CLR (ADR-001 §3.1/§3.6).
    /// <para>
    /// La tabla se extrajo del <c>switch</c> de <c>MSSQLTypeConverter</c> del 1.4
    /// (<c>Providers/MSSQLCommon.cs</c>: <c>NativeTypeExist</c>, <c>NativeType2CLRType</c> y <c>GetLength</c>) y
    /// vive acá como <b>dato portable</b>: ni el reverse del tooling ni el motor de generación declaran una
    /// segunda tabla (ADR-003 §3.5). El orden es estable —por nombre de tipo nativo, ordinal— para que su lectura
    /// sea determinista.
    /// </para>
    /// <para>
    /// No hay acá conocimiento de SQL Server más allá de los nombres de tipo nativo: el quoting de
    /// identificadores, <c>sp_help</c> y las queries de schema son del tooling y del motor.
    /// </para>
    /// </summary>
    public sealed class MSSQLTypeConverter : IDBTypeConverter
    {
        private readonly Dictionary<string, NativeTypeMapping> _byNativeType;

        /// <summary>
        /// 
        /// </summary>
        public MSSQLTypeConverter()
        {
            var entries = new List<NativeTypeMapping>
            {
                new NativeTypeMapping("bigint", typeof(Int64), NativeLengthRule.None),
                new NativeTypeMapping("binary", typeof(Byte[]), NativeLengthRule.AsIs),
                new NativeTypeMapping("bit", typeof(Boolean), NativeLengthRule.None),
                new NativeTypeMapping("char", typeof(String), NativeLengthRule.AsIs),
                new NativeTypeMapping("date", typeof(DateTime), NativeLengthRule.None),
                new NativeTypeMapping("datetime", typeof(DateTime), NativeLengthRule.None),
                new NativeTypeMapping("decimal", typeof(Decimal), NativeLengthRule.AsIs),
                new NativeTypeMapping("float", typeof(Double), NativeLengthRule.None),
                new NativeTypeMapping("image", typeof(Byte[]), NativeLengthRule.None),
                new NativeTypeMapping("int", typeof(Int32), NativeLengthRule.None),
                new NativeTypeMapping("money", typeof(Decimal), NativeLengthRule.None),
                new NativeTypeMapping("nchar", typeof(String), NativeLengthRule.Half),
                new NativeTypeMapping("ntext", typeof(String), NativeLengthRule.Half),
                new NativeTypeMapping("numeric", typeof(Decimal), NativeLengthRule.AsIs),
                new NativeTypeMapping("nvarchar", typeof(String), NativeLengthRule.Half),
                new NativeTypeMapping("real", typeof(Double), NativeLengthRule.None),
                new NativeTypeMapping("smalldatetime", typeof(DateTime), NativeLengthRule.None),
                new NativeTypeMapping("smallint", typeof(Int16), NativeLengthRule.None),
                new NativeTypeMapping("smallmoney", typeof(Decimal), NativeLengthRule.None),
                new NativeTypeMapping("sql_variant", typeof(Object), NativeLengthRule.None),
                new NativeTypeMapping("sysname", typeof(String), NativeLengthRule.Half),
                new NativeTypeMapping("text", typeof(String), NativeLengthRule.AsIs),
                new NativeTypeMapping("timestamp", typeof(Byte[]), NativeLengthRule.AsIs),
                new NativeTypeMapping("tinyint", typeof(Byte), NativeLengthRule.None),
                new NativeTypeMapping("uniqueidentifier", typeof(Guid), NativeLengthRule.None),
                new NativeTypeMapping("varbinary", typeof(Byte[]), NativeLengthRule.AsIs),
                new NativeTypeMapping("varchar", typeof(String), NativeLengthRule.AsIs),
                new NativeTypeMapping("xml", typeof(String), NativeLengthRule.AsIs),
            };

            entries.Sort((left, right) => string.CompareOrdinal(left.NativeType, right.NativeType));
            Table = entries.AsReadOnly();

            _byNativeType = new Dictionary<string, NativeTypeMapping>(StringComparer.Ordinal);
            foreach (var entry in entries)
            {
                _byNativeType.Add(entry.NativeType, entry);
            }
        }

        /// <summary>
        /// La tabla del mapeo, de solo lectura y en orden estable por nombre de tipo nativo.
        /// </summary>
        public IReadOnlyList<NativeTypeMapping> Table { get; }

        /// <inheritdoc />
        public Boolean NativeTypeExist(string nativeType)
        {
            return nativeType != null && _byNativeType.ContainsKey(nativeType.ToLowerInvariant());
        }

        /// <inheritdoc />
        public Type NativeType2CLRType(string nativeType)
        {
            NativeTypeMapping entry;
            if (nativeType == null || !_byNativeType.TryGetValue(nativeType.ToLowerInvariant(), out entry!))
            {
                throw new Exceptions.UnknownDBTypeExeception();
            }

            return entry.ClrType;
        }

        /// <inheritdoc />
        public Nullable<int> GetLength(string nativeType, int nativeLength)
        {
            NativeTypeMapping entry;
            if (nativeType == null || !_byNativeType.TryGetValue(nativeType.ToLowerInvariant(), out entry!))
            {
                throw new Exceptions.UnknownDBTypeExeception();
            }

            switch (entry.LengthRule)
            {
                case NativeLengthRule.AsIs:
                    return nativeLength;
                case NativeLengthRule.Half:
                    return nativeLength == -1 ? -1 : nativeLength / 2;
                default:
                    return null;
            }
        }
    }
}
