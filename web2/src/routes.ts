export default class Routes {
    static readonly base = "/";
    static readonly login = "/login";
    static readonly signup = "/signup";
    static readonly dashboard = "/dashboard";
    static readonly createGame = "/games/create";

    static lobby = (gameId : number) => `/games/${gameId}/lobby`;
    static readonly lobbyPattern = "/games/:gameId/lobby";

    static play = (gameId : number) => `/games/${gameId}/play`;
    static readonly playPattern = "/games/:gameId/play";

    static diplomacy = (gameId : number) => `/games/${gameId}/diplomacy`;
    static readonly diplomacyPattern = "/games/:gameId/diplomacy";
}
