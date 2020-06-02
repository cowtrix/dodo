import { connect } from "react-redux"
import { search, event } from "../domain"
import { Dodo } from "./dodo"

const { searchSetCurrentLocation } = search.actions
const { eventTypesGet } = event.actions

const mapDispatchToProps = dispatch => ({
	startup: () => {
		searchSetCurrentLocation(dispatch)
		eventTypesGet(dispatch)
	}
})

export const DodoConnected = connect(null, mapDispatchToProps)(Dodo)
