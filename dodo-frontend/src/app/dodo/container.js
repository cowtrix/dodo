import { connect } from "react-redux"
import { localGroups, search } from "../domain/index"
import { Dodo } from "./dodo"

const { searchSetCurrentLocation } = search.actions

const mapDispatchToProps = dispatch => ({
	startup: () => {
		searchSetCurrentLocation(dispatch)
	}
})

export const DodoConnected = connect(null, mapDispatchToProps)(Dodo)
