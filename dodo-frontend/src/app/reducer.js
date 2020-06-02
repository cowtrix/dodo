import { combineReducers } from "redux"
import { search, requestsReducer, event } from "./domain"
import { reducer as appReducer } from './dodo/redux'


const searchReducer = search.reducer
const eventReducer = event.reducer

export const store = combineReducers({
	app: appReducer,
	domain: combineReducers({
		search: searchReducer,
		requests: requestsReducer,
		events: eventReducer,
	})
})
