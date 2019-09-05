import { Theme } from "./model";

const imagesDir = "../../resources/images";

const whiteHex = "#FFFFFF";
const blackHex = "#000000";
const medGrayHex = "#828282";
const darkGrayHex = "#161616";
const lightGrayHex = "#DCDCDC";

export default class ThemeFactory {
    public static readonly default : Theme = {
        name: "Default",
        colors: {
            background: "white",
            text: "black",
            headerText: "black",
            border: "gainsboro",
            hoverText: "white",
            hoverBackground: "black",
            altRowText: "black",
            altRowBackground: "gainsboro",

            player0: "blue",
            player1: "red",
            player2: "green",
            player3: "orange",
            player4: "brown",
            player5: "teal",
            player6: "magenta",
            player7: "gold",

            cells: {
                //Cell colors must be in hex
                even: whiteHex,
                odd: blackHex,
                center: medGrayHex,

                evenBorder: null,
                oddBorder: null,
                centerBorder: null,

                evenText: blackHex,
                oddText: whiteHex,
                centerText: whiteHex,

                boardBorder: "black",

                selectedColor: "#6AC921", //Green
                selectedIntensity: 0.75,
                selectableColor: "#E5E500", //Yellow
                selectableIntensity: 0.5
            }
        },
        images: {
            pieces: {
                assassin: `${imagesDir}/default/assassin.png`,
                chief: `${imagesDir}/default/chief.png`,
                corpse: `${imagesDir}/default/corpse.png`,
                diplomat: `${imagesDir}/default/diplomat.png`,
                gravedigger: `${imagesDir}/default/gravedigger.png`,
                reporter: `${imagesDir}/default/reporter.png`,
                thug: `${imagesDir}/default/thug.png`
            }
        },
        copy: {
            gameTitle: "Djambi-N",
            centerCellName: "Seat",
            pieces: {
                assassin: "Assassin",
                chief: "Chief",
                corpse: "Corpse",
                diplomat: "Diplomat",
                gravedigger: "Gravedigger",
                reporter: "Reporter",
                thug: "Thug"
            }
        },
        fonts: {
            headerFamily: "Century Gothic, Geneva, sans-serif",
            normalFamily: "Century Gothic, Geneva, sans-serif"
        }
    };

    public static readonly anesto : Theme = {
        name: "Anesto",
        colors: {
            ...ThemeFactory.default.colors,
            background: "oldlace",
            cells: {
                ...ThemeFactory.default.colors.cells,
                //Cell colors must be in hex
                even: medGrayHex,
                odd: medGrayHex,
                center: "#000080", //Navy

                evenBorder: blackHex,
                oddBorder: blackHex,
                centerBorder: whiteHex,

                evenText: blackHex,
                oddText: blackHex,
                centerText: whiteHex,
            }
        },
        images: {
            pieces: {
                assassin: `${imagesDir}/anesto/assassin.png`,
                chief: `${imagesDir}/anesto/chief.png`,
                corpse: `${imagesDir}/anesto/corpse.png`,
                diplomat: `${imagesDir}/anesto/diplomat.png`,
                gravedigger: `${imagesDir}/anesto/gravedigger.png`,
                reporter: `${imagesDir}/anesto/reporter.png`,
                thug: `${imagesDir}/anesto/thug.png`
            }
        },
        copy: {
            gameTitle: "Djambi",
            centerCellName: "Maze",
            pieces: {
                ...ThemeFactory.default.copy.pieces,
                diplomat: "Provocateur",
                gravedigger: "Necromobile",
                reporter: "Journalist",
                thug: "Militant"
            }
        },
        fonts: {
            headerFamily: "Georgia, serif",
            normalFamily: "Georgia, serif"
        }
    }

    public static readonly hotdogtown : Theme = {
        name: "Hotdogtown",
        colors: {
            ...ThemeFactory.default.colors,
            headerText: "indianred",
            background: "lemonchiffon",
            cells: {
                ...ThemeFactory.default.colors.cells,
                odd: "#00FFFF", //cyan
                even: whiteHex,
            }
        },
        images: {
            pieces: {
                assassin: `${imagesDir}/hotdogtown/assassin.png`,
                chief: `${imagesDir}/hotdogtown/chief.png`,
                corpse: `${imagesDir}/hotdogtown/corpse.png`,
                diplomat: `${imagesDir}/hotdogtown/diplomat.png`,
                gravedigger: `${imagesDir}/hotdogtown/gravedigger.png`,
                reporter: `${imagesDir}/hotdogtown/reporter.png`,
                thug: `${imagesDir}/hotdogtown/thug.png`
            }
        },
        copy: {
            gameTitle: "Hotdogtown",
            centerCellName: "Booth",
            pieces: {
                assassin: "Fork",
                chief: "Ketchup",
                corpse: "Hotdog",
                diplomat: "Hugger",
                gravedigger: "Eater",
                reporter: "Fart",
                thug: "Fries"
            }
        },
        fonts: {
            ...ThemeFactory.default.fonts,
            headerFamily: "Comic Sans MS",
        }
    }

    public static readonly void : Theme = {
        ...ThemeFactory.default,
        name: "Void",
        colors: {
            ...ThemeFactory.default.colors,
            background: darkGrayHex,
            text: lightGrayHex,
            headerText: lightGrayHex,
            border: lightGrayHex,
            hoverBackground: lightGrayHex,
            hoverText: darkGrayHex,
            altRowBackground: "#333333",
            altRowText: lightGrayHex,
            cells: {
                ...ThemeFactory.default.colors.cells,
                even: blackHex,
                odd: "#333333",
                center: "#EEEEEE",

                centerBorder: "#DDDDDD",
                evenBorder: "#DDDDDD",
                oddBorder: "#DDDDDD",
            }
        },
        images: {
            pieces: {
                ...ThemeFactory.default.images.pieces,
                chief: `${imagesDir}/void/chief.png`,
                gravedigger: `${imagesDir}/void/gravedigger.png`,
                reporter: `${imagesDir}/void/reporter.png`,
            }
        },
        copy: {
            gameTitle: "Void",
            centerCellName: "Void",
            pieces: {
                assassin: "Hunter",
                chief: "Conduit",
                corpse: "Husk",
                diplomat: "Transporter",
                gravedigger: "Reaper",
                reporter: "Scientist",
                thug: "Zealot"
            }
        }
    }

    public static getThemes() : Map<string, Theme> {
        const themes = [
            ThemeFactory.default,
            ThemeFactory.anesto,
            ThemeFactory.hotdogtown,
            ThemeFactory.void
        ];

        return new Map<string, Theme>(themes.map(t => [t.name, t]));
    }
}