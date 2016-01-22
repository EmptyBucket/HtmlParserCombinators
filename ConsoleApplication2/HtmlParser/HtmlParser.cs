﻿using System.Linq;
using Sprache;

namespace ConsoleApplication2
{
    public static class HtmlParser
    {
        private static Parser<string> Content => Parse.CharExcept('<').Many().Text();

        private static Parser<string> Item => ShortNode.Or(FullNode.Or(Content));

        private static Parser<string> FullNode =>
            from openTag in TagParser.OpenTag
            from content in Item.Token().Many()
            from closeTag in TagParser.CloseTag(openTag)
            select openTag + string.Join(string.Empty, content) + closeTag;

        private static Parser<string> ShortNode => TagParser.OpenCloseTag.Select(tag => tag.ToString());

        private static Parser<string> Node(string tagName) =>
            from openTag in TagParser.OpenTag
            where openTag.Name == tagName
            from content in Item.Token().Many()
            from closeTag in TagParser.CloseTag(openTag)
            select string.Join(string.Empty, content);

        private static Parser<string> Node(Attribute attribute) =>
            from openTag in TagParser.OpenTag
            where ((OpenTag)openTag).Attributes.Any(atr => atr.Key == attribute.Name && atr.Value == attribute.Value)
            from content in Item.Token().Many()
            from closeTag in TagParser.CloseTag(openTag)
            select string.Join(string.Empty, content);

        private static Parser<string> Node(string tagName, Attribute attribute) =>
            from openTag in TagParser.OpenTag
            where openTag.Name == tagName
            where ((OpenTag)openTag).Attributes.Any(atr => atr.Key == attribute.Name && atr.Value == attribute.Value)
            from content in Item.Token().Many()
            from closeTag in TagParser.CloseTag(openTag)
            select string.Join(string.Empty, content);

        public static Parser<string> FindTag(string nameTag, Attribute attribute) =>
            from dirt in Parse.AnyChar.Except(Node(nameTag, attribute)).Many()
            from content in Node(nameTag, attribute)
            select content;

        public static Parser<string> FindTag(Attribute attribute) =>
            from dirt in Parse.AnyChar.Except(Node(attribute)).Many()
            from content in Node(attribute)
            select content;

        public static Parser<string> FindTag(string nameTag) =>
            from dirt in Parse.AnyChar.Except(Node(nameTag)).Many()
            from content in Node(nameTag)
            select content;
    }
}
