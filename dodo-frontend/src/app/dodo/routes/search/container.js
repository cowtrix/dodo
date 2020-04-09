import { connect } from "react-redux"
import { Search } from "./search"
import { search } from "app/domain"

const mapStateToProps = state => ({
	searchResults: search.selectors.searchResultsFiltered(state)
})

const mapDispatchToProps = dispatch => ({
	getSearchResults: params => search.actions.searchGet(dispatch, params)
})

export const SearchConnected = connect(
	mapStateToProps,
	mapDispatchToProps
)(Search)
