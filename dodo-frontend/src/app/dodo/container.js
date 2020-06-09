import { connect } from "react-redux"
import { search, resources } from "../domain"
import { Dodo } from "./dodo"

const { searchGet } = search.actions
const { eventTypesGet } = resources.actions

const mapDispatchToProps = dispatch => ({
	startup: () => {
		eventTypesGet(dispatch)
		searchGet(dispatch, {})
	}
})

export const DodoConnected = connect(null, mapDispatchToProps)(Dodo)
