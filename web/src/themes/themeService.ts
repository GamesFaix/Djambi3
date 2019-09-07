import { CellView, CellType, CellState, CellHighlight } from "../viewModel/board/model";
import { PieceKind } from "../api/model";
import { Dispatch } from "redux";
import * as StoreDisplay from '../store/display';
import ThemeFactory from "./themeFactory";
import LocalStorageService from "../utilities/localStorageService";
import { Theme } from './model';

export default class ThemeService {

    //#region Theme initialization

    public static changeTheme(themeName : string, dispatch : Dispatch) : void {
        LocalStorageService.themeName = themeName;
        const theme = ThemeFactory.getThemes().get(themeName);
        dispatch(StoreDisplay.Actions.changeTheme(theme));
        ThemeService.applyToCss(theme);
        ThemeService.loadThemeImages(theme, dispatch);
    }

    private static applyToCss(theme : Theme) : void {
        const s = document.documentElement.style;
        const c  = theme.colors;

        s.setProperty("--background-color", c.background);
        s.setProperty("--text-color", c.text);
        s.setProperty("--header-text-color", c.headerText);
        s.setProperty("--border-color", c.border);
        s.setProperty("--hover-text-color", c.hoverText);
        s.setProperty("--hover-background-color", c.hoverBackground);
        s.setProperty("--alt-row-background-color", c.altRowBackground);
        s.setProperty("--alt-row-text-color", c.altRowText);

        s.setProperty("--player-color-0", c.player0);
        s.setProperty("--player-color-1", c.player1);
        s.setProperty("--player-color-2", c.player2);
        s.setProperty("--player-color-3", c.player3);
        s.setProperty("--player-color-4", c.player4);
        s.setProperty("--player-color-5", c.player5);
        s.setProperty("--player-color-6", c.player6);
        s.setProperty("--player-color-7", c.player7);

        s.setProperty("--font-family", theme.fonts.normalFamily);
        s.setProperty("--header-font-family", theme.fonts.headerFamily);
    }

    private static loadThemeImages(theme : Theme, dispatch : Dispatch) : void {
        const kinds = [
            PieceKind.Assassin,
            PieceKind.Chief,
            PieceKind.Corpse,
            PieceKind.Diplomat,
            PieceKind.Gravedigger,
            PieceKind.Reporter,
            PieceKind.Thug
        ];
        kinds.forEach(k => ThemeService.createPieceImage(theme, k, dispatch));
    }

    private static createPieceImage(theme : Theme, kind : PieceKind, dispatch : Dispatch) : HTMLImageElement {
        const image = new (window as any).Image() as HTMLImageElement;
        image.src = ThemeService.getPieceImagePath(theme, kind);
        image.onload = () => dispatch(StoreDisplay.Actions.pieceImageLoaded(kind, image));
        return image;
    }

    public static getPieceImagePath(theme : Theme, kind : PieceKind) : string {
        const i = theme.images.pieces;
        switch (kind) {
            case PieceKind.Assassin: return i.assassin;
            case PieceKind.Chief: return i.chief;
            case PieceKind.Corpse: return i.corpse;
            case PieceKind.Diplomat: return i.diplomat;
            case PieceKind.Gravedigger: return i.gravedigger;
            case PieceKind.Reporter: return i.reporter;
            case PieceKind.Thug: return i.thug;
            default: throw "Invalid piece kind: " + kind;
        }
    }

    //#endregion

    public static loadSavedTheme(dispatch : Dispatch) : void {
        return ThemeService.changeTheme(ThemeService.getSavedThemeName(), dispatch);
    }

    private static getSavedThemeName() : string {
        let name = LocalStorageService.themeName;
        return name ? name : ThemeFactory.default.name;
    }

    //#region Colors

    public static getPlayerColor(theme : Theme, playerColorId : number) : string {
        const c = theme.colors;
        switch(playerColorId) {
            case 0: return c.player0;
            case 1: return c.player1;
            case 2: return c.player2;
            case 3: return c.player3;
            case 4: return c.player4;
            case 5: return c.player5;
            case 6: return c.player6;
            case 7: return c.player7;
            default: throw "Unsupported player color id: " + playerColorId;
        }
    }

    public static getCellColor(theme : Theme, cell : CellView) : string {
        const c = theme.colors.cells;
        if (cell.state === CellState.Selected) {
            return c.selectedColor;
        }
        switch(cell.type){
            case CellType.Center: return c.center;
            case CellType.Even: return c.even;
            case CellType.Odd: return c.odd;
            default: throw "Unsupported celltype: " + cell.type;
        }
    }

    public static getCellBorderColor(theme : Theme, type : CellType) {
        const c = theme.colors.cells;
        switch(type){
            case CellType.Center: return c.centerBorder;
            case CellType.Even: return c.evenBorder;
            case CellType.Odd: return c.oddBorder;
            default: throw "Unsupported celltype: " + type;
        }
    }

    public static getCellTextColor(theme : Theme, type : CellType) {
        const c = theme.colors.cells;
        switch(type){
            case CellType.Center: return c.centerText;
            case CellType.Even: return c.evenText;
            case CellType.Odd: return c.oddText;
            default: throw "Unsupported celltype: " + type;
        }
    }

    //#endregion

    //#region Copy

    public static getPieceName(theme : Theme, kind : PieceKind) : string {
        const c = theme.copy.pieces;
        switch (kind) {
            case PieceKind.Assassin: return c.assassin;
            case PieceKind.Chief: return c.chief;
            case PieceKind.Corpse: return c.corpse;
            case PieceKind.Diplomat: return c.diplomat;
            case PieceKind.Gravedigger: return c.gravedigger;
            case PieceKind.Reporter: return c.reporter;
            case PieceKind.Thug: return c.thug;
            default: throw "Invalid piece kind: " + kind;
        }
    }

    //#endregion
}