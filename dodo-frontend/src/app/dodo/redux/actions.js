import { CENTER_MAP, MENU_OPEN } from './action-types'

export const setCenterMap = (payload) => ({
	type: CENTER_MAP,
	payload,
})

export const setMenuOpen = (payload) => ({
	type: MENU_OPEN,
	payload,
})


