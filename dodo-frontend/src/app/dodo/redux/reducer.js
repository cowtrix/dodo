import { CENTER_MAP } from './action-types'

const initialState = {
	centerMap: false,
}

export const reducer = (state = initialState, action) => {
	switch (action.type) {
		case (CENTER_MAP) : {
			return ({
				...state,
				centerMap: action.payload
			})
		}
		default: return state
	}
}

