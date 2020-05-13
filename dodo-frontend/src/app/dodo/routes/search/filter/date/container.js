import { connect } from "react-redux"
import { Date } from "./date"

import { selectors, actions } from "app/domain/search"

const mapStateToProps = state => ({
	withinStartDate: selectors.withinStartDate(state),
	withinEndDate: selectors.withinEndDate(state)
})

const mapDispatchToProps = dispatch => ({
	updateDate: (withinStartDate, withinEndDate) => {
		dispatch(actions.searchFilterDate({ withinStartDate, withinEndDate }))
	}
})

export const DateConnected = connect(mapStateToProps, mapDispatchToProps)(Date)
