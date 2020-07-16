import { connect } from "react-redux"
import { resources, user } from "app/domain"

import { Dodo } from "./dodo"

const { eventTypesGet } = resources.actions
const { getLoggedInUser } = user.actions
const { fetchingUser } = user.selectors

const mapStateToProps = state => ({
	fetchingUser: fetchingUser(state)
})

const mapDispatchToProps = dispatch => ({
	startup: () => {
		eventTypesGet(dispatch)
		getLoggedInUser(dispatch)
	}
})

export const DodoConnected = connect(mapStateToProps, mapDispatchToProps)(Dodo)
