using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using OpenQA.Selenium;

namespace SeleniumToolkit
{
    public class XpathExpression : IDisposable
    {
        private readonly StringBuilder _expression;
        private bool _hasOperator;
        private bool _hasAttribute;
        private readonly string _nodeSelector;
        private static readonly string[] _operators = new string[] { " and ", " or " };

        /// <summary>
        /// Creates a new xpath expression for the specified tag
        /// </summary>
        /// <param name="tagname">Name of the tag to look for</param>
        /// <param name="nodeSelector">Start point to look for the node, default global</param>
        public XpathExpression(string tagname, string nodeSelector = "//")
        {
            _expression = new StringBuilder();
            _expression.Append(tagname);
            _nodeSelector = nodeSelector;

            _hasOperator = false;
            _hasAttribute = false;
        }

        /// <summary>
        /// Creates an expression object using the * to look for any element
        /// <param name="nodeSelector">Start point to look for the node, default global</param>
        /// </summary>
        public static XpathExpression FromAny(string nodeSelector = "//")
        {
            return new XpathExpression("*", nodeSelector);
        }

        /// <summary>
        /// Creates an expression object with the given tagname and nodeSelector
        /// </summary>
        /// <param name="tagname">The element tagname to look for</param>
        /// <param name="nodeSelector">Start point to look for the node, default global</param>
        public static XpathExpression From(string tagname, string nodeSelector = "//")
        {
            return new XpathExpression(tagname, nodeSelector);
        }

        /// <summary>
        /// Creates an expression object using the ./ selector to get a direct children of the element
        /// </summary>
        /// <param name="tagname">The element tagname to look for</param>
        public static XpathExpression FromChild(string tagname)
        {
            return new XpathExpression(tagname, "./");
        }

        /// <summary>
        /// Creates an expression object using the .// selector to get any children of the element
        /// </summary>
        /// <param name="tagname">The element tagname to look for</param>
        public static XpathExpression FromDescendant(string tagname)
        {
            return new XpathExpression(tagname, ".//");
        }

        /// <summary>
        /// Get the string representation of the curren xpath expression
        /// </summary>
        public string GetExpression()
        {
            string newExpression = _nodeSelector + _expression.ToString();
            if (_hasAttribute)
                newExpression += "]";

            return newExpression;
        }

        /// <summary>
        /// Get the string representation of the curren xpath expression with the specified index
        /// </summary>
        public string GetExpression(int index)
        {
            string newExpression = _nodeSelector + _expression.ToString();
            if (_hasAttribute)
                newExpression += "]";

            return $"({newExpression})[{index}]";
        }

        public string GetPartialExpression()
        {
            string newExpression = _expression.ToString();
            if (_hasAttribute)
                newExpression += "]";

            return newExpression;
        }

        /// <summary>
        /// Creates an expression with the expecified attributeName and value
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="attributeValue">The value to look for the attribute</param>
        /// <param name="useNameSpace">If true it will prefix the attribute with namespace axis</param>
        /// <param name="caseSensitive">If false and attributeValue is not null then evaluate the attribute value as lowercase</param>
        public XpathExpression WhereAttribute(string attributeName, string attributeValue = null, bool useNameSpace = false, bool caseSensitive = true)
        {
            ExecuteDefaultAttributeOperation();

            if (attributeValue == null)
                Append("@" + attributeName);
            else
                ExecuteTextOperation("normalize-space({0})={1}", attributeName, attributeValue, useNameSpace, caseSensitive);

            _hasOperator = false;
            return this;
        }

        /// <summary>
        /// Creates an expression with the expecified attributeName and value
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="attributeValue">The value to look for the attribute</param>
        /// <param name="useNameSpace">If true it will prefix the attribute with namespace axis</param>
        /// <param name="caseSensitive">If false and attributeValue is not null then evaluate the attribute value as lowercase</param>
        public XpathExpression WhereNotAttribute(string attributeName, string attributeValue = null, bool useNameSpace = false, bool caseSensitive = true)
        {
            ExecuteDefaultAttributeOperation();

            if (attributeValue == null)
                Append("@" + attributeName);
            else
                ExecuteTextOperation("not(normalize-space({0})={1})", attributeName, attributeValue, useNameSpace, caseSensitive);

            _hasOperator = false;
            return this;
        }

        /// <summary>
        /// Creates an expression that validates any attribute that have the expecified attribute value
        /// </summary>
        /// <param name="attributeValue">The value to look for inside attributes</param>
        /// <param name="useNameSpace">If true it will prefix the attribute with namespace axis</param>
        public XpathExpression WhereAnyAttribute(string attributeValue = null, bool useNamespace = false)
        {
            ExecuteDefaultAttributeOperation();
            Append(useNamespace ? AddNamespaceToAttribute("*") : "@*");
            Append("=");
            Append(SanatizeQuotes(attributeValue));

            _hasOperator = false;
            return this;
        }

        /// <summary>
        /// Creates an expression that validates any attribute that do not have the expecified attribute value
        /// </summary>
        /// <param name="attributeValue">The value to look for inside attributes</param>
        /// <param name="useNameSpace">If true it will prefix the attribute with namespace axis</param>
        public XpathExpression WhereAnyAttributeNot(string attributeValue = null, bool useNamespace = false)
        {
            ExecuteDefaultAttributeOperation();
            Append("not(");
            Append(useNamespace ? AddNamespaceToAttribute("*") : "@*");
            Append("=");
            Append(SanatizeQuotes(attributeValue));
            Append(")");

            _hasOperator = false;
            return this;
        }

        /// <summary>
        /// Creates an expression for attributes with number and look for one that is greater than the especified attribute value
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="attributeValue">The value as number to compare</param>
        /// <param name="useNameSpace">If true it will prefix the attribute with namespace axis</param>
        public XpathExpression WhereAttributeGreaterThan<T>(string attributeName, T attributeValue, bool useNameSpace = false) where T : IComparable<T>
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("{0} > '{1}' ", useNameSpace ? AddNamespaceToAttribute(attributeName) : "@" + attributeName, attributeValue));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression for attributes with number and look for one that is greater or equal than the especified attribute value
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="attributeValue">The value as number to compare</param>
        /// <param name="useNameSpace">If true it will prefix the attribute with namespace axis</param>
        public XpathExpression WhereAttributeGreaterOrEqualThan<T>(string attributeName, T attributeValue, bool useNameSpace = false) where T : IComparable<T>
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("{0} >= '{1}' ", useNameSpace ? AddNamespaceToAttribute(attributeName) : "@" + attributeName, attributeValue));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression for attributes with number and look for one that is lower than the especified attribute value
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="attributeValue">The value as number to compare</param>
        /// <param name="useNameSpace">If true it will prefix the attribute with namespace axis</param>
        public XpathExpression WhereAttributeLowerThan<T>(string attributeName, T attributeValue, bool useNameSpace = false) where T : IComparable<T>
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("{0} < '{1}' ", useNameSpace ? AddNamespaceToAttribute(attributeName) : "@" + attributeName, attributeValue));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression for attributes with number and look for one that is lower or equal than the especified attribute value
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="attributeValue">The value as number to compare</param>
        /// <param name="useNameSpace">If true it will prefix the attribute with namespace axis</param>
        public XpathExpression WhereAttributeLowerOrEqualThan<T>(string attributeName, T attributeValue, bool useNameSpace = false) where T : IComparable<T>
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("{0} <= '{1}' ", useNameSpace ? AddNamespaceToAttribute(attributeName) : "@" + attributeName, attributeValue));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression for attributes that contains the expecified text
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="textValue">The text to use</param>
        /// <param name="useNameSpace">If true it will prefix the attribute with namespace axis</param>
        /// <param name="caseSensitive">If false and attributeValue is not null then evaluate the attribute value as lowercase</param>
        public XpathExpression WhereAttributeContains(string attributeName, string textValue, bool useNameSpace = false, bool caseSensitive = true)
        {
            ExecuteDefaultAttributeOperation();
            ExecuteTextOperation("contains({0},{1})", attributeName, textValue, useNameSpace, caseSensitive);
            return this;
        }

        /// <summary>
        /// Creates an expression for attributes that do not contains the expecified text
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="textValue">The text to use</param>
        /// <param name="useNameSpace">If true it will prefix the attribute with namespace axis</param>
        /// <param name="caseSensitive">If false and attributeValue is not null then evaluate the attribute value as lowercase</param>
        public XpathExpression WhereAttributeNotContains(string attributeName, string textValue, bool useNameSpace = false, bool caseSensitive = true)
        {
            ExecuteDefaultAttributeOperation();
            ExecuteTextOperation("not(contains({0},{1}))", attributeName, textValue, useNameSpace, caseSensitive);
            return this;
        }

        /// <summary>
        /// Creates an expression for attributes that starts with the expecified text
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="textValue">The text to use</param>
        /// <param name="useNameSpace">If true it will prefix the attribute with namespace axis</param>
        /// <param name="caseSensitive">If false and attributeValue is not null then evaluate the attribute value as lowercase</param>
        public XpathExpression WhereAttributeStartsWith(string attributeName, string textValue, bool useNameSpace = false, bool caseSensitive = true)
        {
            ExecuteDefaultAttributeOperation();
            ExecuteTextOperation("starts-with(normalize-space({0}),{1})", attributeName, textValue, useNameSpace, caseSensitive);
            return this;
        }

        /// <summary>
        /// Creates an expression for attributes that do not starts with the expecified text
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="textValue">The text to use</param>
        /// <param name="useNameSpace">If true it will prefix the attribute with namespace axis</param>
        /// <param name="caseSensitive">If false and attributeValue is not null then evaluate the attribute value as lowercase</param>
        public XpathExpression WhereAttributeNotStartsWith(string attributeName, string textValue, bool useNameSpace = false, bool caseSensitive = true)
        {
            ExecuteDefaultAttributeOperation();
            ExecuteTextOperation("not(starts-with(normalize-space({0}),{1}))", attributeName, textValue, useNameSpace, caseSensitive);
            return this;
        }

        /// <summary>
        /// Creates an expression for attributes that ends with the expecified text
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="textValue">The text to use</param>
        /// <param name="useNameSpace">If true it will prefix the attribute with namespace axis</param>
        /// <param name="caseSensitive">If false and attributeValue is not null then evaluate the attribute value as lowercase</param>
        public XpathExpression WhereAttributeEndsWith(string attributeName, string textValue, bool useNameSpace = false, bool caseSensitive = true)
        {
            ExecuteDefaultAttributeOperation();
            ExecuteTextOperation("ends-with(normalize-space({0}),{1})", attributeName, textValue, useNameSpace, caseSensitive);
            return this;
        }

        /// <summary>
        /// Creates an expression for attributes that do not ends with the expecified text
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="textValue">The text to use</param>
        /// <param name="useNameSpace">If true it will prefix the attribute with namespace axis</param>
        /// <param name="caseSensitive">If false and attributeValue is not null then evaluate the attribute value as lowercase</param>
        public XpathExpression WhereAttributeNotEndsWith(string attributeName, string textValue, bool useNameSpace = false, bool caseSensitive = true)
        {
            ExecuteDefaultAttributeOperation();
            ExecuteTextOperation("not(ends-with(normalize-space({0}),{1}))", attributeName, textValue, useNameSpace, caseSensitive); ;
            return this;
        }

        /// <summary>
        /// Creates an expression for to look for the inner tag text that contains the expecified text value *only the direct text value*
        /// </summary>
        /// <param name="textValue">The text to use</param>
        /// <param name="caseSensitive">If false and attributeValue is not null then evaluate the attribute value as lowercase</param>
        public XpathExpression WhereTextEqual(string textValue, bool caseSensitive = true)
        {
            ExecuteDefaultAttributeOperation();
            ExecuteTextOperation("{0}={1}", "normalize-space(text()[1])", textValue, false, caseSensitive, isAttribute: false);
            return this;
        }

        /// <summary>
        /// Creates an expression to look for the inner tag text to match with the expecified text value
        /// </summary>
        /// <param name="textValue">The text to use</param>
        /// <param name="caseSensitive">If false and attributeValue is not null then evaluate the attribute value as lowercase</param>
        public XpathExpression WhereAllTextEqual(string textValue, bool caseSensitive = true)
        {
            ExecuteDefaultAttributeOperation();
            ExecuteTextOperation("{0}={1}", ".", textValue, false, caseSensitive, isAttribute: false);
            return this;
        }

        /// <summary>
        /// Creates an expression for to look for the inner tag text that contains the expecified text value *only the direct text value*
        /// </summary>
        /// <param name="textValue">The text to use</param>
        /// <param name="caseSensitive">If false and attributeValue is not null then evaluate the attribute value as lowercase</param>
        public XpathExpression WhereTextNotEqual(string textValue, bool caseSensitive = true)
        {
            ExecuteDefaultAttributeOperation();
            ExecuteTextOperation("not({0}={1})", "normalize-space(text()[1])", textValue, false, caseSensitive, isAttribute: false);
            return this;
        }

        /// <summary>
        /// Creates an expression to look for the inner tag text that do not match with the expecified text value
        /// </summary>
        /// <param name="textValue">The text to use</param>
        /// <param name="caseSensitive">If false and attributeValue is not null then evaluate the attribute value as lowercase</param>
        public XpathExpression WhereAllTextNotEqual(string textValue, bool caseSensitive = true)
        {
            ExecuteDefaultAttributeOperation();
            ExecuteTextOperation("not({0}={1})", ".", textValue, false, caseSensitive, isAttribute: false);
            return this;
        }

        /// <summary>
        /// Creates an expression for to look for the inner tag number greater than the expecified text value
        /// </summary>
        /// <param name="number">Number to compare</param>
        public XpathExpression WhereTextGreaterThan<T>(T number) where T : IComparable<T>
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("number(text()) > {0}", number));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression for to look for the inner tag number greater or equal than the expecified text value
        /// </summary>
        /// <param name="number">Number to compare</param>
        public XpathExpression WhereTextGreaterOrEqualThan<T>(T number) where T : IComparable<T>
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("number(text()) >= {0}", number));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression for to look for the inner tag number lower than the expecified text value
        /// </summary>
        /// <param name="number">Number to compare</param>
        public XpathExpression WhereTextLowerThan<T>(T number) where T : IComparable<T>
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("number(text()) < {0}", number));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression for to look for the inner tag number lower or equal than the expecified text value
        /// </summary>
        /// <param name="number">Number to compare</param>
        public XpathExpression WhereTextLowerOrEqualThan<T>(T number) where T : IComparable<T>
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("number(text()) <= {0}", number));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression for to look for the inner tag text that contains the expecified text value *only the direct text value*
        /// </summary>
        /// <param name="textValue">Text value to look for</param>
        /// <param name="caseSensitive">If false and attributeValue is not null then evaluate the attribute value as lowercase</param>
        public XpathExpression WhereTextContains(string textValue, bool caseSensitive = true)
        {
            ExecuteDefaultAttributeOperation();
            ExecuteTextOperation("contains({0},{1})", "text()[1]", textValue, false, caseSensitive, isAttribute: false);
            return this;
        }

        /// <summary>
        /// Creates an expression for to look for the inner tag text that contains the expecified text value
        /// </summary>
        /// <param name="textValue">Text value to look for</param>
        /// <param name="caseSensitive">If false and attributeValue is not null then evaluate the attribute value as lowercase</param>
        public XpathExpression WhereAllTextContains(string textValue, bool caseSensitive = true)
        {
            ExecuteDefaultAttributeOperation();
            ExecuteTextOperation("contains({0},{1})", ".", textValue, false, caseSensitive, isAttribute: false);
            return this;
        }

        /// <summary>
        /// Creates an expression for to look for the inner tag text that do not contains the expecified text value *only the direct text value*
        /// </summary>
        /// <param name="textValue">Text value to look for</param>
        /// <param name="caseSensitive">If false and attributeValue is not null then evaluate the attribute value as lowercase</param>
        public XpathExpression WhereTextNotContains(string textValue, bool caseSensitive = true)
        {
            ExecuteDefaultAttributeOperation();
            ExecuteTextOperation("not(contains({0},{1}))", "text()[1]", textValue, false, caseSensitive, isAttribute: false);
            return this;
        }

        /// <summary>
        /// Creates an expression for to look for the inner tag text that do not contains the expecified text value
        /// </summary>
        /// <param name="textValue">Text value to look for</param>
        /// <param name="caseSensitive">If false and attributeValue is not null then evaluate the attribute value as lowercase</param>
        public XpathExpression WhereAllTextNotContains(string textValue, bool caseSensitive = true)
        {
            ExecuteDefaultAttributeOperation();
            ExecuteTextOperation("not(contains({0},{1}))", ".", textValue, false, caseSensitive, isAttribute: false);
            return this;
        }

        /// <summary>
        /// Creates an expression for to look for the inner tag text that starts with the expecified text value
        /// </summary>
        /// <param name="textValue">Text value to look for</param>
        /// <param name="caseSensitive">If false and attributeValue is not null then evaluate the attribute value as lowercase</param>
        public XpathExpression WhereTextStartsWith(string textValue, bool caseSensitive = true)
        {
            ExecuteDefaultAttributeOperation();
            ExecuteTextOperation("starts-with({0},{1})", "text()[1]", textValue, false, caseSensitive, isAttribute: false);
            return this;
        }

        /// <summary>
        /// Creates an expression for to look for the inner tag text that do not starts with the expecified text value
        /// </summary>
        /// <param name="textValue">Text value to look for</param>
        /// <param name="caseSensitive">If false and attributeValue is not null then evaluate the attribute value as lowercase</param>
        public XpathExpression WhereTextNotStartsWith(string textValue, bool caseSensitive = true)
        {
            ExecuteDefaultAttributeOperation();
            ExecuteTextOperation("not(starts-with({0},{1}))", "text()[1]", textValue, false, caseSensitive, isAttribute: false);
            return this;
        }

        /// <summary>
        /// Creates an expression for to look for the inner tag text that ends with the expecified text value
        /// </summary>
        /// <param name="textValue">Text value to look for</param>
        /// <param name="caseSensitive">If false and attributeValue is not null then evaluate the attribute value as lowercase</param>
        public XpathExpression WhereTextEndsWith(string textValue, bool caseSensitive = true)
        {
            ExecuteDefaultAttributeOperation();
            ExecuteTextOperation("ends-with({0},{1})", "text()[1]", textValue, false, caseSensitive, isAttribute: false);
            return this;
        }

        /// <summary>
        /// Creates an expression for to look for the inner tag text that do not starts with the expecified text value
        /// </summary>
        /// <param name="textValue">Text value to look for</param>
        /// <param name="caseSensitive">If false and attributeValue is not null then evaluate the attribute value as lowercase</param>
        public XpathExpression WhereTextNotEndsWith(string textValue, bool caseSensitive = true)
        {
            ExecuteDefaultAttributeOperation();
            ExecuteTextOperation("not(ends-with({0},{1}))", "text()[1]", textValue, false, caseSensitive, isAttribute: false);
            return this;
        }

        /// <summary>
        /// Creates an expression for the current tag name that look for one that does not have an inner text
        /// </summary>
        public XpathExpression WhereTextIsEmpty()
        {
            ExecuteDefaultAttributeOperation();
            Append("not(contains(text(), '(default)'))");
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression for the current tag name that look for one that have an inner text
        /// </summary>
        public XpathExpression WhereTextIsNotEmpty()
        {
            ExecuteDefaultAttributeOperation();
            Append("contains(text(), '(default)')");
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to validate that the length of the inner string of the current tag is equal to the expecified one
        /// </summary>
        /// <param name="length">The length of the inner tag text</param>
        public XpathExpression WhereTextLength(int length)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("string-length() = {0}", length));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to validate that the length of the inner string of the current tag is not equal to the expecified one
        /// </summary>
        /// <param name="length">The length of the inner tag text</param>
        public XpathExpression WhereNotTextLength(int length)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("not(string-length() = {0})", length));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to validate that the length of the inner string of the current tag is greater than the expecified one
        /// </summary>
        /// <param name="length">The length of the inner tag text</param>
        public XpathExpression WhereTextLengthGreaterThan(int length)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("string-length() > {0}", length));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to validate that the length of the inner string of the current tag is greater or equal than the expecified one
        /// </summary>
        /// <param name="length">The length of the inner tag text</param>
        public XpathExpression WhereTextLengthGreaterOrEqualThan(int length)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("string-length() >= {0}", length));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to validate that the length of the inner string of the current tag is lower than the expecified one
        /// </summary>
        /// <param name="length">The length of the inner tag text</param>
        public XpathExpression WhereTextLengthLowerThan(int length)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("string-length() < {0}", length));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to validate that the length of the inner string of the current tag is lower or equal than the expecified one
        /// </summary>
        /// <param name="length">The length of the inner tag text</param>
        public XpathExpression WhereTextLengthLowerOrEqualThan(int length)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("string-length() <= {0}", length));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for an element that match with the expecified position 
        /// </summary>
        /// <param name="position">The position of element</param>
        public XpathExpression WherePosition(int position)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("position() = {0}", position));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for an element that do not match with the expecified position 
        /// </summary>
        /// <param name="position">The position of element</param>
        public XpathExpression WhereNotPosition(int position)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("not(position() = {0})", position));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for an element that is greater than the expecified position 
        /// </summary>
        /// <param name="position">The position of element</param>
        public XpathExpression WherePositionGreaterThan(int position)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("position() > {0}", position));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for an element that is greater or equal than the expecified position 
        /// </summary>
        /// <param name="position">The position of element</param>
        public XpathExpression WherePositionGreaterOrEqualThan(int position)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("position() >= {0}", position));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for an element that is lower than the expecified position 
        /// </summary>
        /// <param name="position">The position of element</param>
        public XpathExpression WherePositionLowerThan(int position)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("position() < {0}", position));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for an element that is lower or equal than the expecified position 
        /// </summary>
        /// <param name="position">The position of element</param>
        public XpathExpression WherePositionLowerOrEqualThan(int position)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("position() <= {0}", position));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for an element in the last position
        /// </summary>
        /// <param name="positionToDecrease">The position to decrease to the last position,this is useful if wanted the last but one element </param>
        public XpathExpression WhereLastPosition(int positionToDecrease)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("last() - {0}", positionToDecrease));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for an element in the last position
        /// </summary>
        public XpathExpression WhereLastPosition()
        {
            ExecuteDefaultAttributeOperation();
            Append("last()");
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for the existence of a parent of an expecified tag
        /// </summary>
        /// <param name="parentTagName">Parent tagname to look for</param>
        public XpathExpression WhereParent(string parentTagName)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("./parent::{0}", parentTagName));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for the no existence of a parent of an expecified tag
        /// </summary>
        /// <param name="parentTagName">Parent tagname to look for</param>
        public XpathExpression WhereNotParent(string parentTagName)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("not(./parent::{0})", parentTagName));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for the existence of a parent of an expecified tag
        /// </summary>
        /// <param name="parentExpression">Parent expression to apply</param>
        public XpathExpression WhereParent(XpathExpression parentExpression)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("./parent::{0}", parentExpression.GetPartialExpression()));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for the no existence of a parent of an expecified tag
        /// </summary>
        /// <param name="parentExpression">Parent expression to apply</param>
        public XpathExpression WhereNotParent(XpathExpression parentExpression)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("not(./parent::{0})", parentExpression.GetPartialExpression()));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for the existence of a child of an expecified tag
        /// </summary>
        /// <param name="childTagName">Child tagname to look for</param>
        public XpathExpression WhereChild(string childTagName)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("./child::{0}", childTagName));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for the no existence of a child of an expecified tag
        /// </summary>
        /// <param name="childTagName">Child tagname to look for</param>
        public XpathExpression WhereNotChild(string childTagName)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("not(./child::{0})", childTagName));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for the existence of a child of an expecified tag
        /// </summary>
        /// <param name="childExpression">Parent expression to apply</param>
        public XpathExpression WhereChild(XpathExpression childExpression)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("./child::{0}", childExpression.GetPartialExpression()));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for the no existence of a child of an expecified tag
        /// </summary>
        /// <param name="childExpression">Parent expression to apply</param>
        public XpathExpression WhereNotChild(XpathExpression childExpression)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("not(./child::{0})", childExpression.GetPartialExpression()));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for the existence of a parent, grandparent etc of an expecified tag
        /// </summary>
        /// <param name="ancestorTagName">Ancestor tagname to look for</param>
        public XpathExpression WhereAncestor(string ancestorTagName)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("./ancestor::{0}", ancestorTagName));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for the no existence of a parent, grandparent etc of an expecified tag
        /// </summary>
        /// <param name="ancestorTagName">Ancestor tagname to look for</param>
        public XpathExpression WhereNotAncestor(string ancestorTagName)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("not(./ancestor::{0})", ancestorTagName));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for the existence of a parent, grandparent etc of an expecified tag
        /// </summary>
        /// <param name="ancestorExpression">Parent expression to apply</param>
        public XpathExpression WhereAncestor(XpathExpression ancestorExpression)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("./ancestor::{0}", ancestorExpression.GetPartialExpression()));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for the no existence of a parent, grandparent etc of an expecified tag
        /// </summary>
        /// <param name="ancestorExpression">Parent expression to apply</param>
        public XpathExpression WhereNotAncestor(XpathExpression ancestorExpression)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("not(./ancestor::{0})", ancestorExpression.GetPartialExpression()));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for the existence of a child, grandchild etc of an expecified tag
        /// </summary>
        /// <param name="desdendantTagName">Ancestor tagname to look for</param>
        public XpathExpression WhereDescendant(string desdendantTagName)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("./descendant::{0}", desdendantTagName));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for the no existence of a child, grandchild etc of an expecified tag
        /// </summary>
        /// <param name="desdendantTagName">Ancestor tagname to look for</param>
        public XpathExpression WhereNotDescendant(string desdendantTagName)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("not(./descendant::{0})", desdendantTagName));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for the existence of a child, grandchild etc of an expecified tag
        /// </summary>
        /// <param name="descendantExpression">Parent expression to apply</param>
        public XpathExpression WhereDescendant(XpathExpression descendantExpression)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("./descendant::{0}", descendantExpression.GetPartialExpression()));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for the no existence of a child, grandchild etc of an expecified tag
        /// </summary>
        /// <param name="descendantExpression">Parent expression to apply</param>
        public XpathExpression WhereNotDescendant(XpathExpression descendantExpression)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("not(./descendant::{0})", descendantExpression.GetPartialExpression()));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for a previous sibling of the current tag element
        /// </summary>
        /// <param name="siblingTagName">Sibling tagname to look for</param>
        public XpathExpression WherePrecedingSibling(string siblingTagName)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("./preceding-sibling::{0}", siblingTagName));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for a previous sibling that not exist of the current tag element
        /// </summary>
        /// <param name="siblingTagName">Sibling tagname to look for</param>
        public XpathExpression WhereNotPrecedingSibling(string siblingTagName)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("not(./preceding-sibling::{0})", siblingTagName));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for a previous sibling of the current tag element
        /// </summary>
        /// <param name="siblingExpression">Sibling expression to apply</param>
        public XpathExpression WherePrecedingSibling(XpathExpression siblingExpression)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("./preceding-sibling::{0}", siblingExpression.GetPartialExpression()));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for a previous sibling that not exist of the current tag element
        /// </summary>
        /// <param name="siblingExpression">Sibling expression to apply</param>
        public XpathExpression WhereNotPrecedingSibling(XpathExpression siblingExpression)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("not(./preceding-sibling::{0})", siblingExpression.GetPartialExpression()));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for the next sibling of the current tag element
        /// </summary>
        /// <param name="siblingTagName">Sibling tagname to look for</param>
        public XpathExpression WhereFollowingSibling(string siblingTagName)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("./following-sibling::{0}", siblingTagName));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for a the next sibling that do not exist of the current tag element
        /// </summary>
        /// <param name="siblingTagName">Sibling tagname to look for</param>
        public XpathExpression WhereNotFollowingSibling(string siblingTagName)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("not(./following-sibling::{0})", siblingTagName));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for the next sibling of the current tag element
        /// </summary>
        /// <param name="siblingExpression">Sibling expression to apply</param>
        public XpathExpression WhereFollowingSibling(XpathExpression siblingExpression)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("./following-sibling::{0}", siblingExpression.GetPartialExpression()));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to look for a the next sibling that do not exist of the current tag element
        /// </summary>
        /// <param name="siblingExpression">Sibling expression to apply</param>
        public XpathExpression WhereNotFollowingSibling(XpathExpression siblingExpression)
        {
            ExecuteDefaultAttributeOperation();
            Append(string.Format("not(./following-sibling::{0})", siblingExpression.GetPartialExpression()));
            _hasOperator = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to get the parent of an expecified tag
        /// </summary>
        /// <param name="parentTagName">Parent tagname to look for</param>
        public XpathExpression GetParent(string parentTagName)
        {
            ShowInvalidOperationError();

            if (_hasAttribute)
                Append("]");

            AddSubElement(string.Format("parent::{0}", parentTagName));
            _hasAttribute = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to get the parent of an expecified tag
        /// </summary>
        /// <param name="parentExpression">Ancestor expression to apply</param>
        public XpathExpression GetParent(XpathExpression parentExpression)
        {
            ShowInvalidOperationError();

            if (_hasAttribute)
                Append("]");

            AddSubElement(string.Format("parent::{0}", parentExpression.GetPartialExpression()));
            _hasAttribute = parentExpression._hasAttribute;

            return this;
        }

        /// <summary>
        /// Creates an expression to get the child of an expecified tag
        /// </summary>
        /// <param name="parentTagName">Child tagname to look for</param>
        public XpathExpression GetChild(string childTagName)
        {
            ShowInvalidOperationError();

            if (_hasAttribute)
                Append("]");

            AddSubElement(string.Format("child::{0}", childTagName));
            _hasAttribute = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to get the child of an expecified tag
        /// </summary>
        /// <param name="childExpression">Ancestor expression to apply</param>
        public XpathExpression GetChild(XpathExpression childExpression)
        {
            ShowInvalidOperationError();

            if (_hasAttribute)
                Append("]");

            AddSubElement(string.Format("child::{0}", childExpression.GetPartialExpression()));
            _hasAttribute = childExpression._hasAttribute;

            return this;
        }

        /// <summary>
        /// Creates an expression to get the Ancestor of an expecified tag
        /// </summary>
        /// <param name="ancestorTagName">Parent tagname to look for</param>
        public XpathExpression GetAncestor(string ancestorTagName)
        {
            ShowInvalidOperationError();

            if (_hasAttribute)
                Append("]");

            AddSubElement(string.Format("ancestor::{0}", ancestorTagName));
            _hasAttribute = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to get the Ancestor of an expecified tag
        /// </summary>
        /// <param name="ancestorExpression">Ancestor expression to apply</param>
        public XpathExpression GetAncestor(XpathExpression ancestorExpression)
        {
            ShowInvalidOperationError();

            if (_hasAttribute)
                Append("]");

            AddSubElement(string.Format("ancestor::{0}", ancestorExpression.GetPartialExpression()));
            _hasAttribute = ancestorExpression._hasAttribute;

            return this;
        }

        /// <summary>
        /// Creates an expression to get the Descendant of an expecified tag
        /// </summary>
        /// <param name="desdendantTagName">Parent tagname to look for</param>
        public XpathExpression GetDescendant(string desdendantTagName)
        {
            ShowInvalidOperationError();

            if (_hasAttribute)
                Append("]");

            AddSubElement(string.Format("descendant::{0}", desdendantTagName));
            _hasAttribute = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to get the Descendant of an expecified tag
        /// </summary>
        /// <param name="descendantExpression">Ancestor expression to apply</param>
        public XpathExpression GetDescendant(XpathExpression descendantExpression)
        {
            ShowInvalidOperationError();

            if (_hasAttribute)
                Append("]");

            AddSubElement(string.Format("descendant::{0}", descendantExpression.GetPartialExpression()));
            _hasAttribute = descendantExpression._hasAttribute;

            return this;
        }

        /// <summary>
        /// Creates an expression to get the previous sibling of an expecified tag
        /// </summary>
        /// <param name="siblingTagName">Sibling tagname to look for</param>
        public XpathExpression GetPrecedingSibling(string siblingTagName)
        {
            ShowInvalidOperationError();

            if (_hasAttribute)
                Append("]");

            AddSubElement(string.Format("preceding-sibling::{0}", siblingTagName));
            _hasAttribute = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to get the previous sibling  of an expecified tag
        /// </summary>
        /// <param name="siblingExpression">Sibling expression to apply</param>
        public XpathExpression GetPrecedingSibling(XpathExpression siblingExpression)
        {
            ShowInvalidOperationError();

            if (_hasAttribute)
                Append("]");

            AddSubElement(string.Format("preceding-sibling::{0}", siblingExpression.GetPartialExpression()));
            _hasAttribute = siblingExpression._hasAttribute;

            return this;
        }

        /// <summary>
        /// Creates an expression to get the next sibling of an expecified tag
        /// </summary>
        /// <param name="siblingTagName">Sibling tagname to look for</param>
        public XpathExpression GetFollowingSibling(string siblingTagName)
        {
            ShowInvalidOperationError();

            if (_hasAttribute)
                Append("]");

            AddSubElement(string.Format("following-sibling::{0}", siblingTagName));
            _hasAttribute = false;

            return this;
        }

        /// <summary>
        /// Creates an expression to get the next sibling of an expecified tag
        /// </summary>
        /// <param name="siblingExpression">Sibling expression to apply</param>
        public XpathExpression GetFollowingSibling(XpathExpression siblingExpression)
        {
            ShowInvalidOperationError();

            if (_hasAttribute)
                Append("]");

            AddSubElement(string.Format("following-sibling::{0}", siblingExpression.GetPartialExpression()));
            _hasAttribute = siblingExpression._hasAttribute;

            return this;
        }


        /// <summary>
        /// Return the inner text of the element
        /// </summary>
        /// <returns></returns>
        public XpathExpression GetText()
        {
            return AddSubElement("text()");
        }

        /// <summary>
        /// Look for another element with condition satisfied the current xpath expression
        /// </summary>
        /// <param name="tagname">The new element to look for</param>
        public XpathExpression AddSubElement(string tagname)
        {
            ShowInvalidOperationError();

            if (_hasAttribute)
                Append("]");

            Append(string.Format(@"/{0}", tagname));
            _hasAttribute = false;

            return this;
        }

        /// <summary>
        /// Adds the and operator to an expression *It is the default if ommitted*
        /// </summary>
        public XpathExpression And()
        {
            //If expresion doent have an operator and already has an attribute then apply the new condition
            if (!_hasOperator && _hasAttribute)
            {
                _hasOperator = true;
                _expression.Append(_operators[0]);
            }
            return this;
        }

        /// <summary>
        /// Adds the or operator to an expression *It if ommitted it will add the and operator*
        /// </summary>
        public XpathExpression Or()
        {
            //If expresion doent have an operator and already has an attribute then apply the new condition
            if (!_hasOperator && _hasAttribute)
            {
                _hasOperator = true;
                _expression.Append(_operators[1]);
            }
            return this;
        }

        /// <summary>
        /// Creates a new expression as string with two xpath expression nodes to look for
        /// </summary>
        /// <param name="otherExpression">The new expression join with the current one</param>
        /// <returns></returns>
        public string Union(XpathExpression otherExpression)
        {
            ShowInvalidOperationError();
            return string.Format("{0} | {1}", this.GetExpression(), otherExpression.GetExpression());
        }



        /// <summary>
        /// Add an operator at the end of the expression if doesnt have one
        /// </summary>
        private void AddDefaultOperator()
        {
            if (_hasAttribute && !_hasOperator)
                _expression.Append(_operators[0]);
        }

        private void Append(string value)
        {
            _expression.Append(value);
        }

        private void ShowInvalidOperationError()
        {
            //This error occours if add an And|Or statement without an expression next to it
            if (_hasOperator)
                throw new InvalidOperationException("Cannot add an [and,or] operator without an other expression next to it");
        }

        private void ExecuteTextOperation(string expression, string attributeName, string textValue, bool useNameSpace, bool caseSensitive, bool isAttribute = true)
        {
            string _currentAtribute;
            if (isAttribute)
                _currentAtribute = useNameSpace ? AddNamespaceToAttribute(attributeName) : "@" + attributeName;
            else
                _currentAtribute = attributeName;

            if (caseSensitive)
                Append(string.Format(expression, _currentAtribute, SanatizeQuotes(textValue)));
            else
                Append(string.Format(expression, ApplyLowerCase(_currentAtribute), SanatizeQuotes(textValue.ToLower())));

            _hasOperator = false;
        }

        private void ExecuteDefaultAttributeOperation()
        {
            AddDefaultOperator();

            if (!_hasAttribute)
            {
                _hasAttribute = true;
                Append("[");
            }
        }

        private static string SanatizeQuotes(string text)
        {
            return string.Format(text.Contains("'") ? "\"{0}\"" : "'{0}'", text);
        }

        private static string AddNamespaceToAttribute(string attributeName)
        {
            return string.Format(@"namespace::{0}", attributeName);
        }

        private static string ApplyLowerCase(string text)
        {

            return string.Format(@"translate({0}, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')", text);
        }


        public void Dispose()
        {
            _expression.Clear();
        }
    }

}
