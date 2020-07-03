import { CENTER_MAP, MENU_OPEN } from './action-types'

const initialState = {
	centerMap: false,
	menuOpen: false,
}

export const reducer = (state = initialState, action) => {
	switch (action.type) {
		case (CENTER_MAP) : {
			return ({
				...state,
				centerMap: action.payload
			})
		}
		case (MENU_OPEN) : {
			return ({
				...state,
				menuOpen: action.payload
			})
		}
		default: return state
	}
}

