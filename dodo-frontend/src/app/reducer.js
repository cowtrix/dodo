import { combineReducers } from 'redux'
import {reducerFactory} from "./domain/factories"
import { rebellions } from './domain'

const { actionTypes } = rebellions

export const store = combineReducers({
	domain: combineReducers({
		rebellions: reducerFactory(actionTypes.ALL_REBELLIONS_GET)
	})
})