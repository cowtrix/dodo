import { connect } from "react-redux"
import { resources, user } from "app/domain"

import { Dodo } from "./dodo"

const { eventTypesGet } = resources.actions
const { privacyPolicy, rebelAgreement } = resources.selectors
const { getLoggedInUser } = user.actions

const mapStateToProps = state => ({
	privacyPolicy: privacyPolicy(state),
	rebelAgreement: rebelAgreement(state),
});

const mapDispatchToProps = dispatch => ({
	startup: () => {
		eventTypesGet(dispatch)
		getLoggedInUser(dispatch)
	}
})

export const DodoConnected = connect(mapStateToProps, mapDispatchToProps)(Dodo)
