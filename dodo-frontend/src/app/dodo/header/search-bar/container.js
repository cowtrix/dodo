import { connect } from "react-redux"
import { SearchBar } from "./search-bar"
import { withRouter } from "react-router-dom"

import { search } from "app/domain"

const { actions, selectors } = search

const mapStateToProps = state => ({
	searchString: selectors.search(state),
	searchResults: selectors.searchResultsFiltered(state)
})

const mapDispatchToProps = dispatch => ({
	setSearch: searchString =>
		actions.searchGet(dispatch, { search: searchString })
})

export const SearchBarConnected = withRouter(
	connect(mapStateToProps, mapDispatchToProps)(SearchBar)
)
