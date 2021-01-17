using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
#pragma warning disable CA1707 // Identifiers should not contain underscores
namespace UAlbion.Base
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Word : ushort
    {
        Zero = 1,
        One = 15,
        Two = 101,
        Three = 117,
        Four = 201,
        Sofa = 501,
        Flesh = 502,
        Sewage = 503,
        Herds = 504,
        Water1 = 505,
        WaterChannels = 506,
        House = 507,
        Architecture = 508,
        Former = 509,
        DjiFadh = 510,
        Magician = 511,
        DjiKas = 512,
        Clan = 513,
        Council1 = 514,
        Sebainah = 515,
        Marriage1 = 516,
        SecondaryPartner = 517,
        Siblings = 518,
        Maturity = 519,
        Fertility = 520,
        Sebai = 521,
        Ritual1 = 522,
        Chosen = 523,
        Trii = 524,
        Stiriik = 525,
        Laws = 526,
        Guild1 = 527,
        Magic1 = 528,
        Goddess = 529,
        Jirinaar = 530,
        Nakiridaani = 531,
        History1 = 532,
        Humans1 = 533,
        CouncilBuilding = 534,
        CityGate1 = 535,
        Shopping = 536,
        Tavern1 = 537,
        Harbor1 = 538,
        Mark = 539,
        Naked = 540,
        Healing1 = 541,
        Help1 = 542,
        Pain = 543,
        Dra = 544,
        Stri = 545,
        Akiir = 546,
        Murder1 = 547,
        Janiis = 548,
        Fasiir = 549,
        Rejira = 550,
        Argim = 551,
        Legends = 552,
        Skorrek = 553,
        Heal = 554,
        Clothes = 555,
        Signature = 556,
        Food = 557,
        Weapons = 558,
        Equipment = 559,
        Buy1 = 560,
        Religion1 = 561,
        Marriage2 = 562,
        Society = 563,
        Iskai1 = 564,
        Kitchen = 565,
        Gridri = 566,
        Fadhiim = 567,
        Child = 568,
        Giria = 569,
        Hunter1 = 570,
        Medicine = 571,
        Hunt = 572,
        Krondir = 573,
        Klirna = 574,
        Training1 = 575,
        Cultures = 576,
        KengetKamulos1 = 577,
        Beloveno1 = 578,
        Umajo1 = 579,
        Trade1 = 580,
        Beloveno2 = 581,
        Writer = 582,
        Seed = 583,
        Grain = 584,
        GuildHouse = 585,
        Triifalai = 586,
        Kriis = 587,
        Bradir = 588,
        TriNadh = 589,
        Han = 590,
        Sira = 591,
        Representative = 592,
        Instructing = 593,
        Discipline = 594,
        Walls = 595,
        Fountain1 = 596,
        Domes = 597,
        Merchant = 598,
        Wania = 599,
        Wares1 = 600,
        Predators = 601,
        Fruit = 602,
        Pottery = 603,
        Rumor = 604,
        Navigator = 605,
        LandLeave = 606,
        Fun = 607,
        Business = 608,
        Family = 609,
        Rabir = 610,
        HouseOfWinds = 611,
        Drink1 = 612,
        Zoomi = 613,
        Trade2 = 614,
        Metal1 = 615,
        HeavenlyMetal = 616,
        Spices = 617,
        Warehouse = 618,
        CattleBreeder = 619,
        Herd = 620,
        Cattle = 621,
        Ceile = 622,
        Trophy = 623,
        FosterCare = 624,
        Attio = 625,
        Gods = 626,
        Danu = 627,
        Lugh = 628,
        Tuath = 629,
        Tribe = 630,
        King = 631,
        TribalKing = 632,
        Warrior1 = 633,
        Poet = 634,
        Trader = 635,
        WeaponSmith = 636,
        Umajo2 = 637,
        Law = 638,
        Druids = 639,
        Magic2 = 640,
        Healers1 = 641,
        Albion = 642,
        Skull1 = 643,
        Grove = 644,
        Oak = 645,
        Drinno = 646,
        Canto1 = 647,
        Iskai2 = 648,
        Klouta = 649,
        Arjano = 650,
        Gratogel = 651,
        Vanello = 652,
        Aballon = 653,
        Mountains = 654,
        Danger1 = 655,
        Tharnos = 656,
        Oibelos = 657,
        Aretha = 658,
        Nemos = 659,
        SeaVoyage = 660,
        Journey = 661,
        StrengthAmulet = 662,
        Sailor = 663,
        Garris = 664,
        Artisan = 665,
        Shapes = 666,
        MagicalItems = 667,
        AWarrior = 668,
        Queen = 669,
        Profession = 670,
        Fruits = 671,
        Bread = 672,
        UsefulObjects = 673,
        Rations1 = 674,
        School = 675,
        Father = 676,
        Teacher = 677,
        Forget = 678,
        Artifacts = 679,
        Bero = 680,
        Library = 681,
        Toronto = 682,
        DDT = 683,
        AI = 684,
        Ned = 685,
        Nugget = 686,
        Indi = 687,
        Joshi = 688,
        FloppyEars = 689,
        Hoika = 690,
        OverC = 691,
        Complex = 692,
        Snoopy = 693,
        Environmentalist = 694,
        Captain1 = 695,
        Captain2 = 696,
        Brandt = 697,
        Data = 698,
        HQ = 699,
        Hofstedt = 700,
        Security1 = 701,
        Government = 702,
        NavigationOfficer = 703,
        Mathematician = 704,
        Flight = 705,
        Landing = 706,
        Dream = 707,
        ServiceDeck = 708,
        ReserveSystems = 709,
        COMRoom = 710,
        Security2 = 711,
        MiningStructures = 712,
        Robot = 713,
        Ore = 714,
        LifeSupport = 715,
        Other = 716,
        Videos = 717,
        MachineTechnician = 718,
        Ladder = 719,
        Segments = 720,
        GovernmentService = 721,
        Basement = 722,
        StorageRoom = 723,
        Mellthas = 724,
        Amulets = 725,
        Unknown726 = 726,
        God = 727,
        Courage = 728,
        Fear = 729,
        Honor = 730,
        LastBattle = 731,
        Pride = 732,
        Fame = 733,
        Steps = 734,
        Brothers = 735,
        Companion = 736,
        Death1 = 737,
        Die = 738,
        Cuain = 739,
        Demons = 740,
        Shrine1 = 741,
        Khunag = 742,
        Battle = 743,
        Prayer = 744,
        Swords = 745,
        Kledo = 746,
        Oqulo = 747,
        Training2 = 748,
        Rituals = 749,
        Spirit = 750,
        Assassins = 751,
        KamulosSleep = 752,
        NewYearsEve = 753,
        Slaves = 754,
        Drugs = 755,
        Schedule = 756,
        Environment = 757,
        Nature = 758,
        SecretPassage = 759,
        ImpureOne = 760,
        Artorn = 761,
        FiveHighPriests = 762,
        BloodOfHonor = 763,
        BattleMaster = 764,
        MagicMaster = 765,
        CeremonyMaster = 766,
        Greeting = 767,
        Jonatharh = 768,
        Cook = 769,
        Possibilities = 770,
        Leader = 771,
        Son = 772,
        Women = 773,
        Kamulos1 = 774,
        Animebona = 775,
        Animenkna = 776,
        Unknown777 = 777,
        Celts = 778,
        Helromier = 779,
        Answer = 780,
        Vercingetorix = 781,
        Catuvellaunus = 782,
        Canto2 = 783,
        Saethar = 784,
        Bathrig = 785,
        UnknownGod = 786,
        Unknown787 = 787,
        Bard = 788,
        Homer = 789,
        Aristotle = 790,
        Broto = 791,
        Ulysses = 792,
        Entity = 793,
        EnlightenedOnes = 794,
        Ens = 795,
        TransportCaves = 796,
        Servant = 797,
        FeetOnTheGround = 798,
        Arrival = 799,
        Meditate = 800,
        Places = 801,
        GoddessFlowers = 802,
        SpellScrolls = 803,
        Affairs = 804,
        Fighter = 805,
        Trenkiriidan = 806,
        FormerBuilding = 807,
        AmuletOfTheGoddess = 808,
        Warniak = 809,
        Song = 810,
        TriifalaiSeed = 811,
        Healing2 = 812,
        Harriet = 813,
        Healers2 = 814,
        Rifrako = 815,
        Frill = 816,
        UmajoKenta = 1001,
        Metal2 = 1002,
        Guilds = 1003,
        Ritual2 = 1004,
        MotherEarth = 1005,
        Wrath = 1006,
        BadLuck = 1007,
        Weaponsmith = 1008,
        Miners = 1009,
        MountainPriests = 1010,
        DiamondPolisher = 1011,
        Equipmentmaker = 1012,
        Price = 1013,
        Tavern2 = 1014,
        TownHall = 1015,
        Healers3 = 1016,
        Healer = 1017,
        Kounos = 1018,
        Srimalinar = 1019,
        Religion2 = 1020,
        Iskai3 = 1021,
        Plants = 1022,
        CityGate2 = 1023,
        Shops = 1024,
        Currency = 1025,
        Climate = 1026,
        Kenget = 1027,
        Kamulos2 = 1028,
        Harbor2 = 1029,
        Ship = 1030,
        Ships = 1031,
        Weather = 1032,
        SkeimaDin = 1033,
        Monopoly = 1034,
        Money = 1035,
        Prisoner = 1036,
        Guard = 1037,
        Drinks = 1038,
        Porenoil = 1039,
        Workshop = 1040,
        Misunderstanding = 1041,
        Innocence = 1042,
        Patient = 1043,
        Misjudgment = 1044,
        Help2 = 1045,
        Gratitude = 1046,
        Something = 1047,
        Favor = 1048,
        Key = 1049,
        Mine = 1050,
        Prison = 1051,
        Experiences = 1052,
        Destinations = 1053,
        NorthernPart = 1054,
        Heat = 1055,
        Agida = 1056,
        SouthernPart = 1057,
        BoulderBelt = 1058,
        Guide = 1059,
        Ohl = 1060,
        Konny = 1061,
        Nelly = 1062,
        Game = 1063,
        Stake = 1064,
        ThrowTheDice = 1065,
        Thunderbolt = 1066,
        ThunderRumble = 1067,
        Thunder = 1068,
        Riot = 1069,
        Mommy = 1070,
        Parents = 1071,
        Search = 1072,
        Danger2 = 1073,
        Consolation = 1074,
        Evil = 1075,
        Zebenno = 1076,
        OrePrices = 1077,
        Developments = 1078,
        TestDome = 1079,
        Mechanisms = 1080,
        TestPersons = 1081,
        Desert = 1082,
        Water2 = 1083,
        Fountain2 = 1084,
        Prices = 1085,
        Buy2 = 1086,
        Wares2 = 1087,
        Defend = 1088,
        Huntsman = 1089,
        Smithy = 1090,
        Offer = 1091,
        MagicWeapons = 1092,
        Repairs = 1093,
        Secret = 1094,
        QueenOfBeauty = 1095,
        FreeGame = 1096,
        Gems = 1097,
        Polish = 1098,
        Exhibition = 1099,
        Mykonou = 1100,
        Members = 1101,
        Collectors = 1102,
        MagicGems = 1103,
        Experience = 1104,
        Trainer = 1105,
        Request = 1106,
        Preparations = 1107,
        Work = 1108,
        Mining = 1109,
        Trade3 = 1110,
        MiningTunnel = 1111,
        Espionage = 1112,
        Attempt = 1113,
        Share = 1114,
        Knowledge = 1115,
        Circumstances = 1116,
        Kossotto = 1117,
        Location = 1118,
        Edjirr = 1119,
        Evidence = 1120,
        Human = 1121,
        Information = 1122,
        Yes = 1123,
        No = 1124,
        Beloveno3 = 1125,
        Arrim = 1126,
        Opinion = 1127,
        Humans2 = 1128,
        Warrior2 = 1129,
        Shrine2 = 1130,
        Taboo = 1131,
        Creatures = 1132,
        Conflict = 1133,
        Rumors = 1134,
        Zerruma = 1135,
        Kontos = 1136,
        War = 1137,
        Gard = 1138,
        Riko = 1139,
        Herras = 1140,
        History2 = 1141,
        Perron = 1142,
        Situation = 1143,
        Assassination = 1144,
        Reward = 1145,
        Discrepancies = 1146,
        Warning = 1147,
        Stubborn = 1148,
        Mood = 1149,
        Discussion = 1150,
        MurderAttempt = 1151,
        Murder2 = 1152,
        Murder3 = 1153,
        Questions = 1154,
        Arrek = 1155,
        Mellthar = 1156,
        Successor = 1157,
        Lately = 1158,
        Who = 1159,
        Council2 = 1160,
        Changes = 1161,
        Connection = 1162,
        Problems = 1163,
        Believe = 1164,
        Magic3 = 1165,
        Plan = 1166,
        Speech = 1167,
        Kariah = 1168,
        Praise = 1169,
        Hint = 1170,
        Brothel = 1171,
        Continent = 1172,
        Theories = 1173,
        Map = 1174,
        Aurino = 1175,
        Sorrow = 1176,
        Hunter2 = 1177,
        PieceOfLand = 1178,
        Kryte = 1179,
        BadNews = 1180,
        TravelRoutes = 1181,
        Pass = 1182,
        CouncilOfTheJust = 1183,
        Sugo = 1184,
        Ramina = 1185,
        Cavern = 1186,
        Caverns = 1187,
        Kritah = 1188,
        Kritahs = 1189,
        MountainPass = 1190,
        Skull2 = 1191,
        Inn = 1192,
        NorthernSide = 1193,
        SouthernSide = 1194,
        Houses = 1195,
        BuildingStyle = 1196,
        Guild2 = 1197,
        CityGate3 = 1198,
        CityGates = 1199,
        MarketSquare = 1200,
        Metalmakers = 1201,
        KengetKamulos2 = 1202,
        ByLand = 1203,
        Sick = 1204,
        Important = 1205,
        Mother = 1206,
        Maini = 1207,
        Consciousness = 1208,
        Dead = 1209,
        Drink2 = 1210,
        Rations2 = 1211,
        OvernightStay = 1212,
        Drirra = 1213,
        Attack = 1214,
        Death2 = 1215,
        MagiciansGuild1 = 1216,
        MagiciansGuild2 = 1217
    }
}
#pragma warning restore CA1707 // Identifiers should not contain underscores
