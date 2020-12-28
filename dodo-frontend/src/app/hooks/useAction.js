import { useDispatch } from 'react-redux'
import { useCallback } from 'react'

export const useAction = (action) => {
  const dispatch = useDispatch()
  return useCallback(
    action.bind(null, dispatch),
    [dispatch]
  )
}