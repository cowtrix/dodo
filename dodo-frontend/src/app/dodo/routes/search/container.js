import { connect } from "react-redux"
import { Search } from "./search"
import { search } from "app/domain"

const { selectors, actions } = search

const mapStateToProps = state => ({
	searchParams: selectors.searchParams(state),
	searchResults: selectors.searchResults(state),
	isFetchingSearch: selectors.isFetching(state)
})

const mapDispatchToProps = dispatch => ({
	getSearchResults: (searchParams) =>
		actions.searchGet(dispatch, searchParams)
})

export const SearchConnected = connect(
	mapStateToProps,
	mapDispatchToProps
)(Search)
