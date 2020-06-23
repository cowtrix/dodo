import { connect } from "react-redux"
import { resources } from "../domain"
import { Dodo } from "./dodo"

const { eventTypesGet } = resources.actions

const mapDispatchToProps = dispatch => ({
	startup: () => {
		eventTypesGet(dispatch)
	}
})

export const DodoConnected = connect(null, mapDispatchToProps)(Dodo)
