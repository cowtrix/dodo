import { combineReducers } from "redux"
import { search, requestsReducer, resources } from "./domain"
import { reducer as appReducer } from './dodo/redux'


const searchReducer = search.reducer
const resourcesReducer = resources.reducer

export const store = combineReducers({
	app: appReducer,
	domain: combineReducers({
		search: searchReducer,
		resources: resourcesReducer,
	}),
	requests: requestsReducer,
})
