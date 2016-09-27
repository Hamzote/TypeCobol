﻿using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using TypeCobol.Compiler.CodeElements.Expressions;

namespace TypeCobol.Compiler.CodeElements
{
    /// <summary>
    /// p433: STRING statement
    /// The STRING statement strings together the partial or complete contents of two or
    /// more data items or literals into one single data item.
    /// One STRING statement can be written instead of a series of MOVE statements.
    /// </summary>
    public class StringStatement : StatementElement, VariableWriter
    {
        public StringStatement() : base(CodeElementType.StringStatement, StatementType.StringStatement) { }

        /// <summary>
        /// identifier-1, literal-1
        /// 
        /// Represents the sending fields.
        /// </summary>
        public StringContentToConcatenate[] StringContentsToConcatenate { get; set; }

        /// <summary>
        /// identifier-3
        /// 
        /// INTO phrase: Identifies the receiving field (identifier-3).
        /// 
        /// Constraints:
        /// - identifier-3 must reference data items described explicitly
        ///   or implicitly as usage DISPLAY, DISPLAY-1, or NATIONAL.
        ///
        /// - identifier-3 must not reference a data item of category numeric-edited,
        /// alphanumeric-edited, or national-edited; an external floating-point data item of
        /// usage DISPLAY, or an external floating-point data item of usage NATIONAL.
        ///
        /// - identifier-3 must not described with the JUSTIFIED clause.
        /// </summary>
        public ReceivingStorageArea ReceivingField { [CanBeNull] get; set; }
        
        /// <summary>
        /// identifier-4
        /// 
        /// POINTER phrase
        /// Points to a character position in the receiving field. The pointer field
        /// indicates a relative alphanumeric character position, DBCS character
        /// position, or national character position when the receiving field is of usage
        /// DISPLAY, DISPLAY-1, or NATIONAL, respectively.
        /// 
        /// identifier-4
        /// Represents the pointer field. identifier-4 must be large enough to
        /// contain a value equal to the length of the receiving field plus 1.
        /// You must initialize identifier-4 to a nonzero value before execution
        /// of the STRING statement begins.
        ///  
        /// identifier-4 must not be described with the symbol P in its PICTURE
        /// character-string.
        /// </summary>
        public ReceivingStorageArea CharacterPositionInReceivingField {[CanBeNull] get; set; }
                
        /// <summary>
        /// Debug string
        /// </summary>
        public override string ToString()
        {
            if (StringContentsToConcatenate == null && ReceivingField == null && CharacterPositionInReceivingField == null)
            {
                return base.ToString();
            }
            else
            {
                var sb = new StringBuilder(base.ToString());
                if (StringContentsToConcatenate != null)
                {
                    sb.Append("- variables to concat =");
                    foreach (var statementWhat in StringContentsToConcatenate)
                    {
                        sb.Append(" ").Append(statementWhat);
                    }
                }

                if (ReceivingField != null)
                {
                    sb.AppendLine(" into = " + ReceivingField);
                }

                if (CharacterPositionInReceivingField != null)
                {
                    sb.AppendLine(" pointer = " + CharacterPositionInReceivingField);
                }
                return sb.ToString();
            }
        }

        private List<QualifiedName> _variables;
        private IDictionary<QualifiedName, object> _variablesWritten;

        public IList<QualifiedName> Variables
        {
            get {
                if (_variables == null)
                {
                    _variables = new List<QualifiedName>();
                    if (ReceivingField != null)
                    {
                        _variables.Add(((Named) ReceivingField.StorageArea).QualifiedName);
                    }
                }
                return _variables;
            }
        }

        public IDictionary<QualifiedName, object> VariablesWritten
        {
            get
            {
                if (_variablesWritten == null)
                {
                    _variablesWritten = new Dictionary<QualifiedName, object>();
                    if (ReceivingField != null)
                    {
                        _variablesWritten.Add(((Named)ReceivingField.StorageArea).QualifiedName, StringContentsToConcatenate);
                    }
                }
                return _variablesWritten;
            }
        }

        public bool IsUnsafe { get { return false; }  }
    }

	public class StringContentToConcatenate
	{
        /// <summary>
        /// identifier-1, literal-1
        /// Represents the sending fields.
        /// </summary>
		public Variable[] SendingFields { get; set; }

        /// <summary>
        /// DELIMITED BY phrase
        /// Sets the limits of the string.
        /// SIZE Transfers the complete sending area.
        /// </summary>
        public SyntaxProperty<bool> IsDelimitedbySize { get; set; }

        /// <summary>
        /// identifier-2, literal-2
        /// Are delimiters; that is, characters that delimit the data to be
        /// transferred.
        /// </summary>
        public Variable DelimiterCharacters { get; set; }
		
        /// <summary>
        /// Debug string
        /// </summary>
        public override string ToString()
        {
            if (SendingFields == null && IsDelimitedbySize == null && DelimiterCharacters == null)
            {
                return base.ToString();
            }
            else
            {
                var sb = new StringBuilder("");
                if (SendingFields != null)
                {
                    foreach (var sendingField in SendingFields)
                    {
                        sb.Append(' ');
                        sb.Append(sendingField);
                    }
                    sb.Append("   ");
                }

                if (DelimiterCharacters != null)
                {
                    sb.AppendLine(" delimited by " + DelimiterCharacters);
                }

				if (IsDelimitedbySize != null) sb.AppendLine(" delimited by Size");

                return sb.ToString();
            }
        }

    }
}
