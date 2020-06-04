import { connect } from "react-redux"
import { search, resources } from "../domain"
import { Dodo } from "./dodo"

const { searchSetCurrentLocation } = search.actions
const { eventTypesGet } = resources.actions

const mapDispatchToProps = dispatch => ({
	startup: () => {
		searchSetCurrentLocation(dispatch)
		eventTypesGet(dispatch)
	}
})

export const DodoConnected = connect(null, mapDispatchToProps)(Dodo)
