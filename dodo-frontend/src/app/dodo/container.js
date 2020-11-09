import { connect } from "react-redux"
import { resources, user } from "app/domain"

import { Dodo } from "./dodo"

const { eventTypesGet } = resources.actions
const { privacyPolicy } = resources.selectors
const { getLoggedInUser } = user.actions

const mapStateToProps = state => ({
	privacyPolicy: privacyPolicy(state)
});

const mapDispatchToProps = dispatch => ({
	startup: () => {
		eventTypesGet(dispatch)
		getLoggedInUser(dispatch)
	}
})

export const DodoConnected = connect(mapStateToProps, mapDispatchToProps)(Dodo)
