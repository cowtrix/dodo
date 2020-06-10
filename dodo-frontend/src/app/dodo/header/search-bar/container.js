import { connect } from "react-redux"
import { SearchBar } from "./search-bar"
import { withRouter } from "react-router-dom"

import { search } from "app/domain"
import { actions as appActions } from 'app/dodo/redux'


const { actions, selectors } = search

const mapStateToProps = state => ({
	searchString: selectors.search(state),
})

const mapDispatchToProps = dispatch => ({
	setCenterMap: (centerMap) => dispatch(appActions.setCenterMap(centerMap)),
	search: (searchParams, cb) =>
		actions.searchGet(dispatch, searchParams, cb)
})

export const SearchBarConnected = withRouter(
	connect(mapStateToProps, mapDispatchToProps)(SearchBar)
)
