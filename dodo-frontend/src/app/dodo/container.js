import { connect } from "react-redux"
import { resources, user } from "app/domain"

import { Dodo } from "./dodo"

const { eventTypesGet } = resources.actions
const { getLoggedInUser } = user.actions

const mapDispatchToProps = dispatch => ({
	startup: () => {
		eventTypesGet(dispatch)
		getLoggedInUser(dispatch)
	}
})

export const DodoConnected = connect(null, mapDispatchToProps)(Dodo)
