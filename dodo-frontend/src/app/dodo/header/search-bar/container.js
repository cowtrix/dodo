import { connect } from "react-redux"
import { SearchBar } from "./search-bar"
import { withRouter } from "react-router-dom"

import { search } from "app/domain"

const { actions, selectors } = search

const mapStateToProps = state => ({
	searchString: selectors.search(state),
	searchResults: selectors.searchResults(state),
	searchParams: selectors.searchParams(state)
})

const mapDispatchToProps = dispatch => ({
	search: searchParams =>
		actions.searchGet(dispatch, searchParams)
})

export const SearchBarConnected = withRouter(
	connect(mapStateToProps, mapDispatchToProps)(SearchBar)
)
