import React, { FC } from 'react';
import { IconButton, makeStyles } from '@material-ui/core';
import { Menu as MenuIcon } from '@material-ui/icons';
import { useSelector } from 'react-redux';
import { selectNavigation } from '../../hooks/selectors';
import { toggleDrawer } from '../../utilities/navigation';

const useStyles = makeStyles((theme) => ({
  menuButton: {
    marginRight: theme.spacing(2),
  },
}));

const MenuButton: FC = () => {
  const classes = useStyles();

  const state = useSelector(selectNavigation);
  const isOpen = state.isDrawerOpen;

  const toggle = () => {
    toggleDrawer(!isOpen);
  };

  return (
    <IconButton
      edge="start"
      className={classes.menuButton}
      color="inherit"
      onClick={toggle}
    >
      <MenuIcon />
    </IconButton>
  );
};

export default MenuButton;