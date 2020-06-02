import { connect } from "react-redux"
import { Search } from "./search"
import { search } from "app/domain"
import { actions as appActions, selectors as appSelectors } from 'app/dodo/redux'

const { centerMap } = appSelectors
const { setCenterMap } = appActions

const { selectors, actions } = search

const mapStateToProps = state => ({
	centerMap: centerMap(state),
	searchParams: selectors.searchParams(state),
	searchResults: selectors.searchResults(state),
	isFetchingSearch: selectors.isFetching(state)
})

const mapDispatchToProps = dispatch => ({
	setCenterMap: (centerMap) => dispatch(setCenterMap(centerMap)),
	getSearchResults: (searchParams) =>
		actions.searchGet(dispatch, searchParams)
})

export const SearchConnected = connect(
	mapStateToProps,
	mapDispatchToProps
)(Search)
