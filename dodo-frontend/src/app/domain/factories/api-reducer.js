import { CANCELLED, FAILURE, REQUEST, SUCCESS } from "../constants"

const initialStateFactory = actionType => ({
	isFetching: false,
	hasErrored: false
})

export const apiReducerFactory = actionType => (
	state = initialStateFactory(actionType),
	action
) => {
	switch (action.type) {
		case actionType + REQUEST: {
			return {
				...state,
				isFetching: true,
				hasErrored: false,
				error: undefined
			}
		}
		case actionType + SUCCESS:
		case actionType + CANCELLED: {
			return {
				...state,
				isFetching: false,
				hasErrored: false,
				error: undefined
			}
		}
		case actionType + FAILURE: {
			return {
				...state,
				isFetching: false,
				hasErrored: true,
				error: action.payload
			}
		}
		default:
			return state
	}
}

export const reducerFactory = (actionType, defaultState = []) => (state = defaultState, action) => {
	switch (action.type) {
		case actionType + SUCCESS: {
			return action.payload
		}
		default:
			return state
	}
}
