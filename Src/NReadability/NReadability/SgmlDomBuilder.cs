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
using System.Xml;
using System.Xml.Linq;
using Sgml;
using System.IO;

namespace NReadability
{
  /// <summary>
  /// TODO: comment
  /// </summary>
  public class SgmlDomBuilder
  {
    #region Public methods

    /// <summary>
    /// TODO: comment
    /// </summary>
    public XDocument BuildDocument(string htmlContent)
    {
      if (htmlContent == null)
      {
        throw new ArgumentNullException("htmlContent");
      }

      if (htmlContent.Trim().Length == 0)
      {
        return new XDocument();
      }

      // "trim end" htmlContent to ...</html>$ (codinghorror.com puts some scripts after the </html> - sic!)
      const string htmlEnd = "</html";
      int indexOfHtmlEnd = htmlContent.LastIndexOf(htmlEnd);

      if (indexOfHtmlEnd != -1)
      {
        int indexOfHtmlEndBracket = htmlContent.IndexOf('>', indexOfHtmlEnd);

        if (indexOfHtmlEndBracket != -1)
        {
          htmlContent = htmlContent.Substring(0, indexOfHtmlEndBracket + 1);
        }
      }

      // load the document using sgml reader
      using (var sgmlReader = new SgmlReader())
      {
        sgmlReader.CaseFolding = CaseFolding.ToLower;
        sgmlReader.DocType = "HTML";
        sgmlReader.WhitespaceHandling = WhitespaceHandling.All;

        using (var sr = new StringReader(htmlContent))
        {
          sgmlReader.InputStream = sr;

          var document = XDocument.Load(sgmlReader);

          return document;
        }
      }
    }

    #endregion
  }
}
