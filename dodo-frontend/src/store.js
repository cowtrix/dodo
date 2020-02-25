import { createStore } from 'redux'
import { store as appStore } from './app'
import { composeWithDevTools } from 'redux-devtools-extension'

export const store = createStore(appStore, composeWithDevTools())