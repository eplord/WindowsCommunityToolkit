﻿// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Collections.Generic;
using Microsoft.Toolkit.Uwp.UI.Controls.Markdown.Parse;
using UITestMethodAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.UITestMethodAttribute;

namespace UnitTests.Markdown.Parse
{
    [TestClass]
    public class ListTests : ParseTestBase
    {
        [TestMethod]
        [TestCategory("Parse - block")]
        public void BulletedList_SingleLine()
        {
            AssertEqual("- List",
                new ListBlock { Style = ListStyle.Bulleted }.AddChildren(
                    new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "List" }))));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void BulletedList_Simple()
        {
            AssertEqual(CollapseWhitespace(@"
                before

                - List item 1
                * List item 2
                + List item 3

                after"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before" }),
                new ListBlock { Style = ListStyle.Bulleted }.AddChildren(
                    new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 1" })),
                    new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 2" })),
                    new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 3" }))),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "after" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void BulletedList_BlankLineIsOkay()
        {
            AssertEqual(CollapseWhitespace(@"
                * List item 1

                * List item 2"),
                new ListBlock { Style = ListStyle.Bulleted }.AddChildren(
                    new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 1" })),
                    new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 2" }))));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void BulletedList_WithBlocks()
        {
            AssertEqual(CollapseWhitespace(@"
                * #Header

                 ___
                *  #Not a header

                 ___"),
                new ListBlock { Style = ListStyle.Bulleted }.AddChildren(
                    new ListBlock.ListItemBlock().AddChildren(
                        new HeaderBlock { HeaderLevel = 1 }.AddChildren(new TextRunInline { Text = "Header" }),
                        new HorizontalRuleBlock()),
                    new ListBlock.ListItemBlock().AddChildren(
                        new ParagraphBlock().AddChildren(new TextRunInline { Text = " #Not a header" }),
                        new HorizontalRuleBlock())));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void BulletedList_Nested_Simple()
        {
            AssertEqual(CollapseWhitespace(@"
                - List item 1
                    - Nested item
                + List item 2"),
                new ListBlock().AddChildren(
                    new ListBlock.ListItemBlock().AddChildren(
                        new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 1" }),
                        new ListBlock().AddChildren(
                            new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "Nested item" })))),
                    new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 2" }))));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void BulletedList_Nested_Complex()
        {
            // This is super weird.
            AssertEqual(CollapseWhitespace(@"
                - #Level 1
                - #Level 1
                    - #Level 2
                        - #Level 3
                            - #Level 4  
                level 4, line 2

                     text"),
                new ListBlock().AddChildren(
                    new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "#Level 1" })),
                    new ListBlock.ListItemBlock().AddChildren(
                        new HeaderBlock { HeaderLevel = 1 }.AddChildren(new TextRunInline { Text = "Level 1" }),
                        new ListBlock().AddChildren(
                            new ListBlock.ListItemBlock().AddChildren(
                                new HeaderBlock { HeaderLevel = 1 }.AddChildren(new TextRunInline { Text = "Level 2" }),
                                new ListBlock().AddChildren(
                                    new ListBlock.ListItemBlock().AddChildren(
                                        new ParagraphBlock().AddChildren(new TextRunInline { Text = "#Level 3" }),
                                        new ListBlock().AddChildren(
                                            new ListBlock.ListItemBlock().AddChildren(
                                                new ParagraphBlock().AddChildren(new TextRunInline { Text = "#Level 4\r\nlevel 4, line 2" }))))),
                                new ParagraphBlock().AddChildren(new TextRunInline { Text = "text" }))))));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void BulletedList_Nested_MinSpace()
        {
            // 1 space is the min relative indentation.
            AssertEqual(CollapseWhitespace(@"
                - List item 1
                 - Nested item"),
                new ListBlock().AddChildren(
                    new ListBlock.ListItemBlock().AddChildren(
                        new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 1" }),
                        new ListBlock().AddChildren(
                            new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "Nested item" }))))));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void BulletedList_Nested_MaxSpace()
        {
            // 7 spaces is the max relative indentation for two items.
            AssertEqual(CollapseWhitespace(@"
                - List item 1
                       - Nested item"),
                new ListBlock().AddChildren(
                    new ListBlock.ListItemBlock().AddChildren(
                        new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 1" }),
                        new ListBlock().AddChildren(
                            new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "Nested item" }))))));

            // 11 spaces is the max relative indentation for three items.
            AssertEqual(CollapseWhitespace(@"
                - List item 1
                 - List item 2
                           - List item 3"),
            new ListBlock().AddChildren(
                new ListBlock.ListItemBlock().AddChildren(
                    new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 1" }),
                    new ListBlock().AddChildren(
                        new ListBlock.ListItemBlock().AddChildren(
                            new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 2" }),
                            new ListBlock().AddChildren(
                                new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 3" }))))))));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void BulletedList_Nested_SpaceDifference()
        {
            // This is weird.
            AssertEqual(CollapseWhitespace(@"
                 - List item 1
                - Nested item"),
                new ListBlock().AddChildren(
                    new ListBlock.ListItemBlock().AddChildren(
                        new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 1" }),
                        new ListBlock().AddChildren(
                            new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "Nested item" }))))));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void BulletedList_Nested_Combo()
        {
            AssertEqual(CollapseWhitespace(@"
                   - List item 1
                - List item 2
                - List item 3"),
                new ListBlock().AddChildren(
                    new ListBlock.ListItemBlock().AddChildren(
                        new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 1" }),
                        new ListBlock().AddChildren(
                            new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 2" })),
                            new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 3" }))))));
            AssertEqual(CollapseWhitespace(@"
                   - List item 1
                - List item 2
                  - List item 3"),
                new ListBlock().AddChildren(
                    new ListBlock.ListItemBlock().AddChildren(
                        new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 1" }),
                        new ListBlock().AddChildren(
                            new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 2" })),
                            new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 3" }))))));
            AssertEqual(CollapseWhitespace(@"
                   - List item 1
                - List item 2
                   - List item 3"),
                new ListBlock().AddChildren(
                    new ListBlock.ListItemBlock().AddChildren(
                        new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 1" }),
                        new ListBlock().AddChildren(
                            new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 2" })))),
                    new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 3" }))));
            AssertEqual(CollapseWhitespace(@"
                   - List item 1
                - List item 2
                    - List item 3"),
                new ListBlock().AddChildren(
                    new ListBlock.ListItemBlock().AddChildren(
                        new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 1" }),
                        new ListBlock().AddChildren(
                            new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 2" })),
                            new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 3" }))))));
            AssertEqual(CollapseWhitespace(@"
                   - List item 1
                - List item 2
                     - List item 3"),
                new ListBlock().AddChildren(
                    new ListBlock.ListItemBlock().AddChildren(
                        new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 1" }),
                        new ListBlock().AddChildren(
                            new ListBlock.ListItemBlock().AddChildren(
                                new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 2" }),
                                new ListBlock().AddChildren(
                                    new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 3" }))))))));
            AssertEqual(CollapseWhitespace(@"
                - List item 1
                 - List item 2
                    - List item 3"),
                new ListBlock().AddChildren(
                    new ListBlock.ListItemBlock().AddChildren(
                        new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 1" }),
                        new ListBlock().AddChildren(
                            new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 2" })),
                            new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 3" }))))));
            AssertEqual(CollapseWhitespace(@"
                - List item 1
                 - List item 2
                     - List item 3"),
                new ListBlock().AddChildren(
                    new ListBlock.ListItemBlock().AddChildren(
                        new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 1" }),
                        new ListBlock().AddChildren(
                            new ListBlock.ListItemBlock().AddChildren(
                                new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 2" }),
                                new ListBlock().AddChildren(
                                    new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 3" }))))))));
            AssertEqual(CollapseWhitespace(@"
                - 1
                 - 2
                - 3
                 - 4"),
                new ListBlock { Style = ListStyle.Bulleted }.AddChildren(
                    new ListBlock.ListItemBlock().AddChildren(
                        new ParagraphBlock().AddChildren(new TextRunInline { Text = "1" }),
                        new ListBlock { Style = ListStyle.Bulleted }.AddChildren(
                            new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "2" })))),
                    new ListBlock.ListItemBlock().AddChildren(
                        new ParagraphBlock().AddChildren(new TextRunInline { Text = "3" }),
                        new ListBlock { Style = ListStyle.Bulleted }.AddChildren(
                            new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "4" }))))));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void BulletedList_Nested_Paragraph()
        {
            AssertEqual(CollapseWhitespace(@"
                * 1
                 * 2

                 3"),
            new ListBlock().AddChildren(
                new ListBlock.ListItemBlock().AddChildren(
                    new ParagraphBlock().AddChildren(new TextRunInline { Text = "1" }),
                    new ListBlock().AddChildren(
                        new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "2" }))),
                    new ParagraphBlock().AddChildren(new TextRunInline { Text = "3" }))));

            AssertEqual(CollapseWhitespace(@"
                * 1
                 * 2
                * 3

                     4"),
                new ListBlock().AddChildren(
                    new ListBlock.ListItemBlock().AddChildren(
                        new ParagraphBlock().AddChildren(new TextRunInline { Text = "1" }),
                        new ListBlock().AddChildren(
                            new ListBlock.ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "2" })))),
                    new ListBlock.ListItemBlock().AddChildren(
                        new ParagraphBlock().AddChildren(new TextRunInline { Text = "3" }),
                        new ParagraphBlock().AddChildren(new TextRunInline { Text = " 4" }))));
        }

        // This test is ignored because it is written to pass the "reddit" quirks of markdown.
        // This parser doesn't conform to the reddit quirks, thus they shall not pass.
        [Ignore]
        [TestMethod]
        [TestCategory("Parse - block")]
        public void BulletedList_NestedLists()
        {
            // No blank line means only the last line actually has a bullet.
            AssertEqual(CollapseWhitespace(@"
1. * Ordered list item 1
2. * Bullet 1 in list item 2
    * Bullet 2 in list item 2"),
                new ListBlock { Style = ListStyle.Numbered }.AddChildren(
                    new ListBlock.ListItemBlock().AddChildren(
                        new ParagraphBlock().AddChildren(
                            new TextRunInline { Text = "* Ordered list item 1" })),
                    new ListBlock.ListItemBlock().AddChildren(
                        new ParagraphBlock().AddChildren(
                            new TextRunInline { Text = "* Bullet 1 in list item 2" }),
                        new ListBlock().AddChildren(
                            new ListBlock.ListItemBlock().AddChildren(
                                new ParagraphBlock().AddChildren(
                                    new TextRunInline { Text = "Bullet 2 in list item 2" }))))));

            // But if you put a blank line in there it works.
            AssertEqual(CollapseWhitespace(@"
1. * Ordered list item 1

2. * Bullet 1 in list item 2
    * Bullet 2 in list item 2"),
                new ListBlock { Style = ListStyle.Numbered }.AddChildren(
                    new ListBlock.ListItemBlock().AddChildren(
                        new ListBlock().AddChildren(
                            new ListBlock.ListItemBlock().AddChildren(
                                new ParagraphBlock().AddChildren(
                                    new TextRunInline { Text = "Ordered list item 1" })))),
                    new ListBlock.ListItemBlock().AddChildren(
                        new ListBlock().AddChildren(
                            new ListBlock.ListItemBlock().AddChildren(
                                new ParagraphBlock().AddChildren(
                                    new TextRunInline { Text = "Bullet 1 in list item 2" })),
                            new ListBlock.ListItemBlock().AddChildren(
                                new ParagraphBlock().AddChildren(
                                    new TextRunInline { Text = "Bullet 2 in list item 2" }))))));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void BulletedList_Negative_SpaceRequired()
        {
            // The space is required.
            AssertEqual("-List",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "-List" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void BulletedList_Negative_NewParagraph()
        {
            // Bulleted lists must start on a new paragraph
            AssertEqual(CollapseWhitespace(@"
                before
                * List
                after"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before * List after" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void BulletedList_Negative_TooMuchSpaceToBeNested()
        {
            // 7 spaces is the maximum indentation for two items.
            AssertEqual(CollapseWhitespace(@"
                * a
                        * b"),
                new ListBlock().AddChildren(
                    new ListBlock.ListItemBlock().AddChildren(
                        new ParagraphBlock().AddChildren(
                            new TextRunInline { Text = "a * b" }))));

            // 11 spaces is the maximum indentation for three items.
            AssertEqual(CollapseWhitespace(@"
                * a
                 * b
                            * c"),
                new ListBlock().AddChildren(
                    new ListBlock.ListItemBlock().AddChildren(
                        new ParagraphBlock().AddChildren(
                            new TextRunInline { Text = "a" }),
                        new ListBlock().AddChildren(
                            new ListBlock.ListItemBlock().AddChildren(
                                new ParagraphBlock().AddChildren(
                                    new TextRunInline { Text = "b * c" }))))));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void NumberedList_SingleLine()
        {
            AssertEqual("1. List",
                new ListBlock { Style = ListStyle.Numbered }.AddChildren(
                    new ListBlock.ListItemBlock { Blocks = new List<MarkdownBlock> { new ParagraphBlock().AddChildren(new TextRunInline { Text = "List" }) } }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void NumberedList_Numbering()
        {
            // The numbers are ignored, and they can be any length.
            AssertEqual(CollapseWhitespace(@"
                7. List item 1
                502. List item 2
                502456456456456456456456456456456456. List item 3"),
                new ListBlock { Style = ListStyle.Numbered }.AddChildren(
                    new ListBlock.ListItemBlock { Blocks = new List<MarkdownBlock> { new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 1" }) } },
                    new ListBlock.ListItemBlock { Blocks = new List<MarkdownBlock> { new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 2" }) } },
                    new ListBlock.ListItemBlock { Blocks = new List<MarkdownBlock> { new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 3" }) } }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void NumberedList_Negative_SpaceRequired()
        {
            // A space is required after the dot.
            AssertEqual("1.List", new ParagraphBlock().AddChildren(
                new TextRunInline { Text = "1.List" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void NumberedList_Negative_NoLetters()
        {
            // Only digits can make a numbered list.
            AssertEqual("a. List", new ParagraphBlock().AddChildren(
                new TextRunInline { Text = "a. List" }));
        }
    }
}