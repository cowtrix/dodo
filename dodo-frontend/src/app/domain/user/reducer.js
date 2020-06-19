import { SUCCESS } from '../constants'
import { LOGIN } from './action-types'

export const reducer = (state = {}, action) => {
	switch (action.type) {
		case LOGIN + SUCCESS: {
			return action.payload
		}
		default:
			return state
	}
}