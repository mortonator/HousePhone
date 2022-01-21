using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ListManager : MonoBehaviour
{
    public static ListManager Instance { get; protected set; }

    public int[] listsUsed;

    bool IsWebGL;
    List<List<string>> customLists;
    string[] standardWordList =
        {
        "AFRICA",
"AGENT",
"AIR",
"ALIEN",
"ALPS",
"AMAZON",
"AMBULANCE",
"AMERICA",
"ANGEL",
"ANTARCTICA",
"APPLE",
"ARM",
"ATLANTIS",
"AUSTRALIA",
"AZTC",
"BACK",
"BALL",
"BAND",
"BANK",
"BAR",
"BARK",
"BAT",
"BATTERY",
"BEACH",
"BEAR",
"BEAT",
"BED",
"BEIJING",
"BELL",
"BELT",
"BERLIN",
"BERMUDA",
"BERRY",
"BILL",
"BLOCK",
"BOARD",
"BOLT",
"BOMB",
"BOND",
"BOOM",
"BOOT",
"BOTTLE",
"BOW",
"BOX",
"BRIDGE",
"BRUSH",
"BUCK",
"BUFFALO",
"BUG",
"BUGLE",
"BUTTON",
"CALF",
"CANADA",
"CAP",
"CAPITAL",
"CAR",
"CARD",
"CARROT",
"CASINO",
"CAST",
"CAT",
"CELL",
"CENTAUR",
"CENTER",
"CHAIR",
"CHANGE",
"CHARGE",
"CHECK",
"CHEST",
"CHICK",
"CHINA",
"CHOCOLATE",
"CHURCH",
"CIRCLE",
"CLIFF",
"CLOAK",
"CLUB",
"CODE",
"COLD",
"COMIC",
"COMPOUND",
"CONCERT",
"CONDUCTOR",
"CONTRACT",
"COOK",
"COPPER",
"COTTON",
"COURT",
"COVER",
"CRANE",
"CRASH",
"CRICKET",
"CROSS",
"CROWN",
"CYCLE",
"CZECH",
"DANCE",
"DATE",
"DAY",
"DEATH",
"DECK",
"DEGREE",
"DIAMOND",
"DICE",
"DINOSAUR",
"DISEASE",
"DOCTOR",
"DOG",
"DRAFT",
"DRAGON",
"DRESS",
"DRILL",
"DROP",
"DUCK",
"DWARF",
"EAGLE",
"EGYPT",
"EMBASSY",
"ENGINE",
"ENGLAND",
"EUROPE",
"EYE",
"FACE",
"FAIR",
"FALL",
"FAN",
"FENCE",
"FIELD",
"FIGHTER",
"FIGURE",
"FILE",
"FILM",
"FIRE",
"FISH",
"FLUTE",
"FLY",
"FOOT",
"FORCE",
"FOREST",
"FORK",
"FRANCE",
"GAME",
"GAS",
"GENIUS",
"GERMANY",
"GHOST",
"GIANT",
"GLASS",
"GLOVE",
"GOLD",
"GRACE",
"GRASS",
"GREECE",
"GREEN",
"GROUND",
"HAM",
"HAND",
"HAWK",
"HEAD",
"HEART",
"HELICOPTER",
"HIMALAYAS",
"HOLE",
"HOLLYWOOD",
"HONEY",
"HOOD",
"HOOK",
"HORN",
"HORSE",
"HORSESHOE",
"HOSPITAL",
"HOTEL",
"ICE",
"ICE CREAM",
"INDIA",
"IRON",
"IVORY",
"JACK",
"JAM",
"JET",
"JUPITER",
"KANGAROO",
"KETCHUP",
"KEY",
"KID",
"KING",
"KIWI",
"KNIFE",
"KNIGHT",
"LAB",
"LAP",
"LASER",
"LAWYER",
"LEAD",
"LEMON",
"LEPRECHAUN",
"LIFE",
"LIGHT",
"LIMOUSINE",
"LINE",
"LINK",
"LION",
"LITTER",
"LOCH NESS",
"LOCK",
"LOG",
"LONDON",
"LUCK",
"MAIL",
"MAMMOTH",
"MAPLE",
"MARBLE",
"MARCH",
"MASS",
"MATCH",
"MERCURY",
"MEXICO",
"MICROSCOPE",
"MILLIONAIRE",
"MINE",
"MINT",
"MISSILE",
"MODEL",
"MOLE",
"MOON",
"MOSCOW",
"MOUNT",
"MOUSE",
"MOUTH",
"MUG",
"NAIL",
"NEEDLE",
"NET",
"NEW YORK",
"NIGHT",
"NINJA",
"NOTE",
"NOVEL",
"NURSE",
"NUT",
"OCTOPUS",
"OIL",
"OLIVE",
"OLYMPUS",
"OPERA",
"ORANGE",
"ORGAN",
"PALM",
"PAN",
"PANTS",
"PAPER",
"PARACHUTE",
"PARK",
"PART",
"PASS",
"PASTE",
"PENGUIN",
"PHOENIX",
"PIANO",
"PIE",
"PILOT",
"PIN",
"PIPE",
"PIRATE",
"PISTOL",
"PIT",
"PITCH",
"PLANE,",
"PLASTIC",
"PLATE",
"PLATYPUS",
"PLAY",
"PLOT",
"POINT",
"POISON",
"POLE",
"POLICE",
"POOL",
"PORT",
"POST",
"POUND",
"PRESS",
"PRINCESS",
"PUMPKIN",
"PUPIL",
"PYRAMID",
"QUEEN",
"RABBIT",
"RACKET",
"RAY",
"REVOLUTION",
"RING",
"ROBIN",
"ROBOT",
"ROCK",
"ROME",
"ROOT",
"ROSE",
"ROULETTE",
"ROUND",
"ROW",
"RULER",
"SATELLITE",
"SATURN",
"SCALE",
"SCHOOL",
"SCIENTIST",
"SCORPION",
"SCREEN",
"SCUBA DIVER",
"SEAL",
"SERVER",
"SHADOW",
"SHAKESPEARE",
"SHARK",
"SHIP",
"SHOE",
"SHOP",
"SHOT",
"SINK",
"SKYSCRAPER",
"SLIP",
"SLUG",
"SMUGGLER",
"SNOW",
"SNOWMAN",
"SOCK",
"SOLDIER",
"SOUL",
"SOUND",
"SPACE",
"SPELL",
"SPIDER",
"SPIKE",
"SPINE",
"SPOT",
"SPRING",
"SPY",
"SQUARE",
"STADIUM",
"STAFF",
"STAR",
"STATE",
"STICK",
"STOCK",
"STRAW",
"STREAM",
"STRIKE",
"STRING",
"SUB",
"SUIT",
"SUPERHERO",
"SWING",
"SWITCH",
"TABLE",
"TABLET",
"TAG",
"TAIL",
"TAP",
"TEACHER",
"TELESCOPE",
"TEMPLE",
"THEATER",
"THIEF",
"THUMB",
"TICK",
"TIE",
"TIME",
"TOKYO",
"TOOTH",
"TORCH",
"TOWER",
"TRACK",
"TRAIN",
"TRIANGLE",
"TRIP",
"TRUNK",
"TUBE",
"TURKEY",
"UNDERTAKER",
"UNICORN",
"VACUUM",
"VAN",
"VET",
"WAKE",
"WALL",
"WAR",
"WASHER",
"WASHINGTON",
"WATCH",
"WATER",
"WAVE",
"WEB",
"WELL",
"WHALE",
"WHIP",
"WIND",
"WITCH",
"WORM",
"YARD"
    };

    void Awake()
    {
        Instance = this;

        IsWebGL = Application.platform == RuntimePlatform.WebGLPlayer;

        GetCustomLists();
    }

    void GetCustomLists()
    {
        if (IsWebGL)
            return;

        int count = PlayerPrefs.GetInt("CustomList_Count");
        customLists = new List<List<string>>();
        string newList;

        count--;
        for (; count >= 0; count--)
        {
            newList = PlayerPrefs.GetString("CustomList_" + count);

            if (string.IsNullOrWhiteSpace(newList))
                continue;

            customLists.Add(newList.Split(';').ToList());
        }
    }

    public void AddCustomList(List<string> newList)
    {
        string singleVar = string.Empty;
        foreach (string item in newList)
        {
            singleVar += item + ";";
        }
        singleVar.Remove(singleVar.Length - 1);

        if (IsWebGL)
        {
            customLists.Add(newList);
        }
        else
        {
            PlayerPrefs.SetString("CustomList_" + customLists.Count, singleVar);
            customLists.Add(newList);
            PlayerPrefs.SetInt("CustomList_Count", customLists.Count);
        }
    }
    public void RemoveCustomList(int listIndex)
    {
        if (customLists == null || customLists.Count == 0)
            return;

        if (listIndex != customLists.Count - 1)
        {
            customLists[listIndex] = customLists[customLists.Count - 1];
        }

        customLists.RemoveAt(customLists.Count - 1);
        if (IsWebGL == false)
        {
            PlayerPrefs.SetInt("CustomList_Count", customLists.Count);
            PlayerPrefs.SetString("CustomList_" + (listIndex + 1), string.Empty);
        }
    }

    public List<string> GetRandomWordList(int length)
    {
        List<string> list = new List<string>();

        string newWord;
        int randomListIndex;
        for (int i = length - 1; i >= 0; i--)
        {
            do
            {
                randomListIndex = listsUsed[Random.Range(0, listsUsed.Length)];

                if (randomListIndex == 0)
                    newWord = standardWordList[Random.Range(0, standardWordList.Length)];
                else
                    newWord = customLists[randomListIndex][Random.Range(0, customLists[randomListIndex].Count)];

            } while (list.Contains(newWord));
            list.Add(newWord);
        }
        return list;
    }
}