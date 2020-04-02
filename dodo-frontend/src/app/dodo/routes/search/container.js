import { connect } from 'react-redux'
import { Search } from './search'
import { rebellions, search } from 'app/domain'


const mapStateToProps = state => ({
  searchResults: search.selectors.searchResults(state)
})

const mapDispatchToProps = dispatch => ({
  getSearchResults: (params) => search.actions.searchGet(dispatch, params)
})

export const SearchConnected = connect(mapStateToProps, mapDispatchToProps)(Search)
