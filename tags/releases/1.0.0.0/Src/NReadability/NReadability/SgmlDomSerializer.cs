﻿/*
 * NReadability
 * http://code.google.com/p/nreadability/
 * 
 * Copyright 2010 Marek Stój
 * http://immortal.pl/
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Linq;
using System.Xml.Linq;

namespace NReadability
{
  /// <summary>
  /// A class for serializing a DOM to string.
  /// </summary>
  public class SgmlDomSerializer
  {
    #region Public methods

    /// <summary>
    /// Serializes given DOM (System.Xml.Linq.XDocument object) to a string.
    /// </summary>
    /// <param name="document">System.Xml.Linq.XDocument instance containing the DOM to be serialized.</param>
    /// <param name="prettyPrint">Determines whether the output will be formatted.</param>
    /// <param name="dontIncludeMetaContentTypeElement">Determines whether DOCTYPE will be included at the beginning of the output.</param>
    /// <param name="dontIncludeDocType">Determines whether a meta tag with a content-type specification will be added/replaced in the output.</param>
    /// <returns>Serialized representation of the DOM.</returns>
    public string SerializeDocument(XDocument document, bool prettyPrint, bool dontIncludeMetaContentTypeElement, bool dontIncludeDocType)
    {
      if (!dontIncludeMetaContentTypeElement)
      {
        var documentRoot = document.Root;

        if (documentRoot == null)
        {
          throw new ArgumentException("The document must have a root.");
        }

        if (documentRoot.Name == null || !"html".Equals(documentRoot.Name.LocalName, StringComparison.OrdinalIgnoreCase))
        {
          throw new ArgumentException("The document's root must be an html element.");
        }

        var headElement = documentRoot.GetChildrenByTagName("head").FirstOrDefault();

        if (headElement == null)
        {
          headElement = new XElement("head");
          documentRoot.Add(headElement);
        }

        var metaContentTypeElement =
          (from metaElement in headElement.GetChildrenByTagName("meta")
           where "content-type".Equals(metaElement.GetAttributeValue("http-equiv", ""), StringComparison.OrdinalIgnoreCase)
           select metaElement).FirstOrDefault();

        if (metaContentTypeElement != null)
        {
          metaContentTypeElement.Remove();
        }

        metaContentTypeElement =
          new XElement(
            XName.Get("meta", headElement.Name != null ? (headElement.Name.NamespaceName ?? "") : ""),
            new XAttribute("http-equiv", "Content-Type"),
            new XAttribute("content", "text/html; charset=utf-8"));

        headElement.AddFirst(metaContentTypeElement);
      }

      string result = document.ToString(prettyPrint ? SaveOptions.None : SaveOptions.DisableFormatting);

      if (!dontIncludeDocType)
      {
        result = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\"\r\n\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n" + result;
      }

      return result;
    }

    /// <summary>
    /// Serializes given DOM (System.Xml.Linq.XDocument object) to a string.
    /// </summary>
    /// <param name="document">System.Xml.Linq.XDocument instance containing the DOM to be serialized.</param>
    /// <returns>Serialized representation of the DOM.</returns>
    public string SerializeDocument(XDocument document)
    {
      return SerializeDocument(document, false, false, false);
    }

    #endregion
  }
}
