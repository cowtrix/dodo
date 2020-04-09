import { connect } from "react-redux"
import { localGroups } from "../domain/index"
import { Dodo } from "./dodo"

const { allLocalGroupsGet } = localGroups.actions

const mapDispatchToProps = dispatch => ({
	startup: () => {
		allLocalGroupsGet(dispatch)
	}
})

export const DodoConnected = connect(null, mapDispatchToProps)(Dodo)
