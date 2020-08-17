import React, { FC, useEffect } from 'react';
import { useSelector } from 'react-redux';
import RedirectToSignInIfSignedOut from '../routing/RedirectToSignInIfSignedOut';
import { GamePageProps } from './GamePage';
import { selectActiveGame } from '../../hooks/selectors';
import { loadGame } from '../../controllers/gameController';

const GameLobbyPage: FC<GamePageProps> = ({ gameId }) => {
  const state = useSelector(selectActiveGame);

  useEffect(() => {
    if (state.game === null) {
      loadGame(gameId);
    }
  });

  return (
    <div>
      <RedirectToSignInIfSignedOut />
      {`Game ${gameId} lobby page`}
      <br />
      {state.game ? JSON.stringify(state.game) : ''}
    </div>
  );
};

export default GameLobbyPage;