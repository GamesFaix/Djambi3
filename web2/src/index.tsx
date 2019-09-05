import * as React from 'react';
import { render } from 'react-dom';
import { Provider } from 'react-redux';
import App from './components/app';
import { createStore, applyMiddleware } from 'redux';
import { Router } from 'react-router-dom';
import thunk from 'redux-thunk';
import {history} from './history';
import "./styles/styles.less";
import { ApiClientCore } from './api/clientCore';
import { ApiRequest, ApiResponse, ApiError } from './api/requestModel';
import * as StoreApiClient from './store/apiClient';
import * as StoreRoot from './store/root';
import { SseClientManager } from './utilities/serverSentEvents';

const store = createStore(
    StoreRoot.reducer,
    StoreRoot.defaultState,
    applyMiddleware(thunk)
);

ApiClientCore.init(
    (request: ApiRequest) => store.dispatch(StoreApiClient.Actions.apiRequest(request)),
    (response: ApiResponse) => store.dispatch(StoreApiClient.Actions.apiResponse(response)),
    (error: ApiError) => store.dispatch(StoreApiClient.Actions.apiError(error)),
    () => StoreRoot.getState(store).settings.debug.logApi
);

SseClientManager.init(store);

render(
    <Provider store={store}>
        <Router history={history}>
            <App/>
        </Router>
    </Provider>,
    document.getElementById('root')
);