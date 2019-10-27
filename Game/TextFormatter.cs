﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UAlbion.Formats;
using UAlbion.Formats.AssetIds;
using UAlbion.Formats.Assets;

namespace UAlbion.Game
{
    public class TextFormatter
    {
        public class TextBlock
        {
            public enum TextStyle
            {
                Normal,
                Big,
                Fat,
                High,
                FatAndHigh
            }

            public enum TextAlignment
            {
                Left,
                Center,
                Justified
            }

            public IList<WordId> Words { get; }
            public string Text { get; }
            public CommonColor Color { get; }
        }

        readonly IAssetManager _assets;
        ICharacterSheet _leader;
        ICharacterSheet _subject;
        ICharacterSheet _inventory;
        ICharacterSheet _combatant;
        ICharacterSheet _victim;
        ItemData _weapon;
        GameLanguage _language;

        public TextFormatter(IAssetManager assets, GameLanguage language)
        {
            _assets = assets;
            _language = language;
        }

        public TextFormatter Leader(ICharacterSheet character) { _leader = character; return this; }
        public TextFormatter Subject(ICharacterSheet character) { _subject = character; return this; }
        public TextFormatter Inventory(ICharacterSheet character) { _inventory = character; return this; }
        public TextFormatter Combatant(ICharacterSheet character) { _combatant = character; return this; }
        public TextFormatter Victim(ICharacterSheet character) { _victim = character; return this; }
        public TextFormatter Weapon(ItemData weapon) { _weapon = weapon; return this; }
        public TextFormatter Language(GameLanguage language) { _language = language; return this; }

        IEnumerable<(Token, object)> Substitute(IEnumerable<(Token, object)> tokens, object[] args)
        {
            object active = null;
            int argNumber = 0;
            foreach(var (token, p) in tokens)
            {
                switch(token)
                {
                    case Token.Damage: throw new NotImplementedException();
                    case Token.Me: throw new NotImplementedException();

                    case Token.Class:
                    {
                        if (!(active is ICharacterSheet character))
                            throw new FormatException($"Expected the active item to be a character, was actually {active}");
                        switch (character.Class)
                        {
                            case PlayerClass.Pilot: yield return (Token.Text, _assets.LoadString(SystemTextId.Class_Pilot, _language)); break;
                            case PlayerClass.Scientist: yield return (Token.Text, _assets.LoadString(SystemTextId.Class_Scientist, _language)); break;
                            case PlayerClass.IskaiWarrior: yield return (Token.Text, _assets.LoadString(SystemTextId.Class_Warrior1, _language)); break;
                            case PlayerClass.DjiKasMage: yield return (Token.Text, _assets.LoadString(SystemTextId.Class_DjiKasMage, _language)); break;
                            case PlayerClass.Druid: yield return (Token.Text, _assets.LoadString(SystemTextId.Class_Druid, _language)); break;
                            case PlayerClass.EnlightenedOne: yield return (Token.Text, _assets.LoadString(SystemTextId.Class_EnlightenedOne, _language)); break;
                            case PlayerClass.Technician: yield return (Token.Text, _assets.LoadString(SystemTextId.Class_Technician, _language)); break;
                            case PlayerClass.OquloKamulos: yield return (Token.Text, _assets.LoadString(SystemTextId.Class_OquloKamulos, _language)); break;
                            case PlayerClass.Warrior: yield return (Token.Text, _assets.LoadString(SystemTextId.Class_Warrior2, _language)); break;
                            case PlayerClass.Monster: yield return (Token.Text, "Monster"); break;
                            default: throw new ArgumentOutOfRangeException();
                        }
                        break;
                    }

                    case Token.He:
                    case Token.Him: 
                    case Token.His: 
                    {
                        if (!(active is ICharacterSheet character))
                            throw new FormatException($"Expected the active item to be a character, was actually {active}");

                        var word = (token, character.Gender) switch
                            {
                                (Token.He, Gender.Male) => SystemTextId.Meta_He,
                                (Token.He, Gender.Female) => SystemTextId.Meta_She,
                                (Token.He, Gender.Neuter) => SystemTextId.Meta_ItNominative,
                                (Token.Him, Gender.Male) => SystemTextId.Meta_HimAccusative,
                                (Token.Him, Gender.Female) => SystemTextId.Meta_HerAccusative,
                                (Token.Him, Gender.Neuter) => SystemTextId.Meta_ItAccusative,
                                (Token.His, Gender.Male) => SystemTextId.Meta_His,
                                (Token.His, Gender.Female) => SystemTextId.Meta_Her,
                                (Token.His, Gender.Neuter) => SystemTextId.Meta_Its,
                                _ => throw new NotImplementedException()
                            };

                        yield return (Token.Text, _assets.LoadString(word, _language));
                        break;
                    }

                    case Token.Name: 
                    {
                        if (active is ICharacterSheet character)
                            yield return (Token.Text, character.GetName(_language));

                        if (active is ItemData item)
                            yield return (Token.Text, item.GetName(_language));
                        break;
                    }

                    case Token.Price:
                    {
                        if (!(active is ItemData item))
                            throw new FormatException($"Expected the active item to be an item, was actually {active}");
                        yield return (Token.Text, $"${item.Value/10}.{item.Value % 10}"); // TODO: Does this need extra logic?
                        break;
                    }

                    case Token.Race:
                    {
                        if (!(active is ICharacterSheet character))
                            throw new FormatException($"Expected the active item to be a character, was actually {active}");
                        switch (character.Race)
                        {
                            case PlayerRace.Terran: yield return (Token.Text, _assets.LoadString(SystemTextId.Race_Terran, _language)); break;
                            case PlayerRace.Iskai: yield return (Token.Text, _assets.LoadString(SystemTextId.Race_Iskai, _language)); break;
                            case PlayerRace.Celt: yield return (Token.Text, _assets.LoadString(SystemTextId.Race_Celt, _language)); break;
                            case PlayerRace.KengetKamulos: yield return (Token.Text, _assets.LoadString(SystemTextId.Race_KengetKamulos, _language)); break;
                            case PlayerRace.DjiCantos: yield return (Token.Text, _assets.LoadString(SystemTextId.Race_DjiCantos, _language)); break;
                            case PlayerRace.Mahino: yield return (Token.Text, _assets.LoadString(SystemTextId.Race_Mahino, _language)); break;
                            default: throw new ArgumentOutOfRangeException();
                        }
                        break;
                    }

                    case Token.Sex:
                    {
                        if (!(active is ICharacterSheet character))
                            throw new FormatException($"Expected the active item to be a character, was actually {active}");
                        switch (character.Gender)
                        {
                            case Gender.Male: yield return (Token.Text, "♂"); break;
                            case Gender.Female: yield return (Token.Text, "♀"); break;
                        }

                        break;
                    }

                    case Token.Word:
                    { 
                        WordId? word = _assets.ParseWord((string)p);
                        if(word == null)
                            yield return (Token.Text, p);
                        else
                            yield return (Token.Text, _assets.LoadString(word.Value, _language));
                        break;
                    }

                        // Change context
                    case Token.Combatant: active = _combatant; break;
                    case Token.Inventory: active = _inventory; break;
                    case Token.Leader: active = _leader; break;
                    case Token.Subject: active = _subject; break;
                    case Token.Victim: active = _victim; break;
                    case Token.Weapon: active = _weapon; break;

                    case Token.Parameter:
                        yield return (Token.Text, args[argNumber].ToString());
                        argNumber++;
                        break;

                    default: yield return (token, p); break;
                }
            }
        }

        public string Format(string template, params object[] arguments)
        {
            var tokens = Tokeniser.Tokenise(template).ToList();
            var words = tokens.Where(x => x.Item1 == Token.Word).Select(x => (string)x.Item2);
            var substituted = Substitute(tokens, arguments);
            var sb = new StringBuilder();
            foreach(var (token, p) in substituted)
            {
                if (token != Token.Text)
                    continue;
                sb.Append((string) p);
            }
            return sb.ToString();
        }
    }
}
