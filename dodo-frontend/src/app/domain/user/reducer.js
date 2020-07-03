import { SUCCESS } from '../constants'
import { GET_LOGGED_IN_USER, LOGIN } from './action-types'

export const reducer = (state = {}, action) => {
	switch (action.type) {
		case LOGIN + SUCCESS: {
			return action.payload
		}
		case GET_LOGGED_IN_USER + SUCCESS: {
			return action.payload
		}
		default:
			return state
	}
}