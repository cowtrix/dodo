import { SUCCESS } from '../constants'
import { GET_LOGGED_IN_USER, GET_MY_REBELLION, LOGIN } from './action-types'

export const reducer = (state = {}, action) => {
	switch (action.type) {
		case LOGIN + SUCCESS:
		case GET_LOGGED_IN_USER + SUCCESS:
			return {
				...state,
				...action.payload
			}
		case GET_MY_REBELLION + SUCCESS:
			return {
				...state,
				myRebellion: action.payload
			}
		default:
			return state
	}
}
