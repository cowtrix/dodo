import { useDispatch } from 'react-redux'
import { useCallback } from 'react'

export const useAction = /**
 * Convert an action (dispatch as the first parameter) into a ready-to-use function
 * @param {(dispatch: import('redux').Dispatch<any>, ...args: P) => R} action
 * @return {(...args: P) => R}
 * @template P
 * @template R
 */
 (action) => {
  const dispatch = useDispatch()
  return useCallback(
    action.bind(null, dispatch),
    [dispatch]
  )
}